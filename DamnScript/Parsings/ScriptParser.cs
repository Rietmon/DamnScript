using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Parsings.G4;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Parsings;

public static unsafe class ScriptParser
{
    public static ScriptData ParseScript(string scriptCode)
    {
        var charStream = new AntlrInputStream(scriptCode);
        var lexer = new DamnScriptLexer(charStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new DamnScriptParser(tokenStream);
        var file = parser.file();
        
        var scriptData = new ScriptData();
        var regions = new NativeList<RegionData>(16);
        
        var strings = new NativeList<UnsafeStringPair>(16);
        var context = new ScriptParserContext();
        for (var i = 0; i < file.ChildCount - 1; i++)
        {
            var assembler = new ScriptAssembler();
            context.strings = &strings;
            context.assembler = &assembler;
            
            var region = ParseRegion(file.GetChild(i), context);
            regions.Add(region);
            
            context.assembler->Dispose();
        }
        
        scriptData.regions = regions.ToArrayAlloc();
        regions.Dispose();
        
        scriptData.metadata = new ScriptMetadata(new ConstantsData(strings.ToArrayAlloc()));
        strings.Dispose();
        
        return scriptData;
    }
    
    public static RegionData ParseRegion(IParseTree region, ScriptParserContext context)
    {
        var regionName = region.GetChild(1).GetText();
        var statement = region.GetChild(2);
        
        Parse(statement, context);

        var byteCode = context.assembler->FinishAlloc();
        return new RegionData(String32.FromString(regionName), byteCode);
    }
    
    public static void Parse(IParseTree tree, ScriptParserContext context)
    {
        switch (tree)
        {
            case DamnScriptParser.CallStatementContext callStatement:
                ParseCallStatementBody(callStatement, context);
                return;
            case DamnScriptParser.AdditiveExpressionContext operatorContext:
                ParseAdditiveExpression(operatorContext, context);
                return;
            case DamnScriptParser.IfStatementContext ifStatement:
                ParseIfStatementBody(ifStatement, context);
                return;
            
            case DamnScriptParser.NumberExpressionContext expression:
                AssemblyNumberExpression(expression, context);
                break;
            case DamnScriptParser.StringLiteralContext stringLiteral:
                AssemblyStringLiteral(stringLiteral, context);
                break;
            case DamnScriptParser.AdditiveOperatorContext operatorContext:
                AssemblyAdditiveOperator(operatorContext, context);
                break;
            case DamnScriptParser.MultiplicativeOperatorContext operatorContext:
                AssemblyMultiplicativeOperator(operatorContext, context);
                break;
        }
        
        for (var i = 0; i < tree.ChildCount; i++)
        {
            var child = tree.GetChild(i);
            Parse(child, context);
        }
    }

    public static void ParseCallStatementBody(IParseTree callStatement, ScriptParserContext context)
    {
        var start = callStatement.GetChild(0);
        var keyword = start as DamnScriptParser.KeywordContext;
        var nextIndex = keyword == null ? 0 : 1;
        var methodCall = callStatement.GetChild(nextIndex);
        if (keyword != null)
            AssemblyKeyword(keyword, context);
        AssemblyMethodCall((DamnScriptParser.MethodCallContext)methodCall, context);
    }

    private static void ParseAdditiveExpression(DamnScriptParser.AdditiveExpressionContext operatorContext, ScriptParserContext context)
    {
        for (var i = 0; i < operatorContext.ChildCount; i++)
        {
            var child = operatorContext.GetChild(i);
            if (child is DamnScriptParser.AdditiveOperatorContext)
                Parse(operatorContext.GetChild(++i), context);
            Parse(child, context);
        }
    }
    
    public static void ParseIfStatementBody(DamnScriptParser.IfStatementContext ifStatement, ScriptParserContext context)
    {
    }
    
    public static void AssemblyKeyword(DamnScriptParser.KeywordContext keyword, ScriptParserContext context)
    {
        var text = keyword.GetText();
        var threadParameter = text switch
        {
            "%noawait%" => SetThreadParameters.ThreadParameters.NoAwait,
            _ => SetThreadParameters.ThreadParameters.None
        };
        
        if (threadParameter == SetThreadParameters.ThreadParameters.None)
        {
            Debugging.LogError($"[{nameof(ScriptParser)}] ({AssemblyKeyword}) Unknown keyword: {text}!");
            return;
        }
        
        context.assembler->SetThreadParameters(threadParameter);
    }
    
    public static void AssemblyMethodCall(DamnScriptParser.MethodCallContext methodCall, ScriptParserContext context)
    {
        var identifier = methodCall.GetChild(0);
        for (var i = 0; i < methodCall.ChildCount; i++)
        {
            var child = methodCall.GetChild(i);
            Parse(child, context);
        }
        
        context.assembler->NativeCall(identifier.GetText(), methodCall.ChildCount);
    }
    
    public static void AssemblyNumberExpression(DamnScriptParser.NumberExpressionContext numberExpression, ScriptParserContext context)
    {
        var number = numberExpression.GetText();
        var value = long.Parse(number);
        context.assembler->PushToStack(value);
    }
    
    public static void AssemblyStringLiteral(DamnScriptParser.StringLiteralContext stringLiteral, ScriptParserContext context)
    {
        var text = stringLiteral.GetText();
        var value = UnsafeString.Alloc(text);
        var hash = value->GetHashCode();
        context.strings->Add(new UnsafeStringPair(hash, value));
        context.assembler->PushStringToStack(hash);
    }
    
    public static void AssemblyMultiplicativeOperator(DamnScriptParser.MultiplicativeOperatorContext multiplicativeOperator, ScriptParserContext context)
    {
        var operatorText = multiplicativeOperator.GetText();
        AssemblyOperator(operatorText, context);
    }
    
    public static void AssemblyAdditiveOperator(DamnScriptParser.AdditiveOperatorContext additiveOperator, ScriptParserContext context)
    {
        var operatorText = additiveOperator.GetText();
        AssemblyOperator(operatorText, context);
    }

    public static void AssemblyOperator(string operatorText, ScriptParserContext context)
    {
        var operatorType = operatorText switch
        {
            "+" => ExpressionCall.ExpressionCallType.Add,
            "-" => ExpressionCall.ExpressionCallType.Subtract,
            "*" => ExpressionCall.ExpressionCallType.Multiply,
            "/" => ExpressionCall.ExpressionCallType.Divide,
            "%" => ExpressionCall.ExpressionCallType.Modulo,
            "==" => ExpressionCall.ExpressionCallType.Equal,
            "!=" => ExpressionCall.ExpressionCallType.NotEqual,
            ">" => ExpressionCall.ExpressionCallType.Greater,
            ">=" => ExpressionCall.ExpressionCallType.GreaterOrEqual,
            "<" => ExpressionCall.ExpressionCallType.Less,
            "<=" => ExpressionCall.ExpressionCallType.LessOrEqual,
            "&&" => ExpressionCall.ExpressionCallType.And,
            "||" => ExpressionCall.ExpressionCallType.Or,
            "!" => ExpressionCall.ExpressionCallType.Not,
            _ => ExpressionCall.ExpressionCallType.Invalid
        };
        if (operatorType == ExpressionCall.ExpressionCallType.Invalid)
        {
            Debugging.LogError($"[{nameof(ScriptParser)}] ({AssemblyAdditiveOperator}) Unknown operator: {operatorText}!");
            return;
        }
        context.assembler->ExpressionCall(operatorType);
    }
}