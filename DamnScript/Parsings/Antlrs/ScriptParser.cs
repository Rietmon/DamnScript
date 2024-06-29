using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Parsings.G4;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.VirtualMachines.Assemblers;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Parsings.Antlrs;

internal static unsafe class ScriptParser
{
    public static void ParseScript(string scriptCode, string scriptName, ScriptData* scriptData)
    {
        var charStream = new AntlrInputStream(scriptCode);
        var lexer = new DamnScriptLexer(charStream);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new DamnScriptParser(tokenStream);
        var file = parser.file();
        
        scriptData->name = new String32(scriptName);

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
        
        if (context.isError)
        {
            scriptData->Dispose();
            *scriptData = default; 
            regions.Dispose();
            strings.Dispose();
            return;
        }
        
        scriptData->regions = regions.ToArrayAlloc();
        regions.Dispose();
        
        scriptData->metadata = new ScriptMetadata(new ConstantsData(strings.ToArrayAlloc()));
        strings.Dispose();
    }

    private static RegionData ParseRegion(IParseTree region, ScriptParserContext context)
    {
        var regionName = region.GetChild(1).GetText();
        var statement = region.GetChild(2);
        
        ParseNode(statement, context);

        var byteCode = context.assembler->FinishAlloc();
        return new RegionData(new String32(regionName), byteCode);
    }

    private static void ParseNode(IParseTree tree, ScriptParserContext context)
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
            
            case ErrorNodeImpl errorNode:
                context.isError = true;
                Debugging.LogError($"[{nameof(ScriptParser)}] ({nameof(ParseNode)}) Error node: \"{errorNode.GetText()}\"!" +
                                   $" Token number: {errorNode.SourceInterval.a}");
                return;
        }
        
        if (context.isError)
            return;
        
        for (var i = 0; i < tree.ChildCount; i++)
        {
            var child = tree.GetChild(i);
            ParseNode(child, context);
        }
    }

    private static void ParseCallStatementBody(IParseTree callStatement, ScriptParserContext context)
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
                ParseNode(operatorContext.GetChild(++i), context);
            ParseNode(child, context);
        }
    }

    private static void ParseIfStatementBody(DamnScriptParser.IfStatementContext ifStatement, ScriptParserContext context)
    {
    }

    private static void AssemblyKeyword(DamnScriptParser.KeywordContext keyword, ScriptParserContext context)
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

    private static void AssemblyMethodCall(DamnScriptParser.MethodCallContext methodCall, ScriptParserContext context)
    {
        var identifier = methodCall.GetChild(0);
        for (var i = 0; i < methodCall.ChildCount; i++)
        {
            var child = methodCall.GetChild(i);
            ParseNode(child, context);
        }
        
        context.assembler->NativeCall(identifier.GetText(), methodCall.ChildCount);
    }

    private static void AssemblyNumberExpression(DamnScriptParser.NumberExpressionContext numberExpression, ScriptParserContext context)
    {
        var number = numberExpression.GetText();
        var value = long.Parse(number);
        context.assembler->PushToStack(value);
    }

    private static void AssemblyStringLiteral(DamnScriptParser.StringLiteralContext stringLiteral, ScriptParserContext context)
    {
        var text = stringLiteral.GetText();
        var value = UnsafeString.Alloc(text);
        var hash = value->GetHashCode();
        context.strings->Add(new UnsafeStringPair(hash, value));
        context.assembler->PushStringToStack(hash);
    }

    private static void AssemblyMultiplicativeOperator(DamnScriptParser.MultiplicativeOperatorContext multiplicativeOperator, ScriptParserContext context)
    {
        var operatorText = multiplicativeOperator.GetText();
        AssemblyOperator(operatorText, context);
    }

    private static void AssemblyAdditiveOperator(DamnScriptParser.AdditiveOperatorContext additiveOperator, ScriptParserContext context)
    {
        var operatorText = additiveOperator.GetText();
        AssemblyOperator(operatorText, context);
    }

    private static void AssemblyOperator(string operatorText, ScriptParserContext context)
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