using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Debugs;
using DamnScript.Parsings.G4;
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
        for (var i = 0; i < file.ChildCount - 1; i++)
        {
            var region = ParseRegion(file.GetChild(i));
            scriptData.regions = (region);
        }

        return scriptData;
    }
    
    public static RegionData ParseRegion(IParseTree region)
    {
        var assembler = new ScriptAssembler(1024);
        var regionName = region.GetChild(1).GetText();
        var statement = region.GetChild(2);
        
        Parse(statement, &assembler);

        var byteCode = assembler.Finish();
        return new RegionData(regionName, byteCode);
    }
    
    public static void Parse(IParseTree tree, ScriptAssembler* assembler)
    {
        switch (tree)
        {
            case DamnScriptParser.CallStatementContext callStatement:
                ParseCallStatementBody(callStatement, assembler);
                return;
            case DamnScriptParser.IfStatementContext ifStatement:
                ParseIfStatementBody(ifStatement, assembler);
                break;
            case DamnScriptParser.NumberExpressionContext expression:
                ParseNumberExpression(expression, assembler);
                break;
            case DamnScriptParser.AdditiveExpressionContext operatorContext:
                ParseAdditiveExpression(operatorContext, assembler);
                return;
            case DamnScriptParser.AdditiveOperatorContext operatorContext:
                ParseAdditiveOperator(operatorContext, assembler);
                break;
            case DamnScriptParser.MultiplicativeOperatorContext operatorContext:
                ParseMultiplicativeOperator(operatorContext, assembler);
                break;
        }
        
        for (var i = 0; i < tree.ChildCount; i++)
        {
            var child = tree.GetChild(i);
            Parse(child, assembler);
        }
    }

    public static void ParseCallStatementBody(IParseTree callStatement, ScriptAssembler* assembler)
    {
        var start = callStatement.GetChild(0);
        var keyword = start as DamnScriptParser.KeywordContext;
        var nextIndex = keyword == null ? 0 : 1;
        var methodCall = callStatement.GetChild(nextIndex);
        if (keyword != null)
            ParseKeyword(keyword, assembler);
        ParseMethodCall((DamnScriptParser.MethodCallContext)methodCall, assembler);
    }
    
    public static void ParseKeyword(DamnScriptParser.KeywordContext keyword, ScriptAssembler* assembler)
    {
        var text = keyword.GetText();
        var threadParameter = text switch
        {
            "%noawait%" => SetThreadParameters.ThreadParameters.NoAwait,
            _ => SetThreadParameters.ThreadParameters.None
        };
        
        if (threadParameter == SetThreadParameters.ThreadParameters.None)
            Debugging.LogError($"[{nameof(ScriptParser)}] ({ParseKeyword}) Unknown keyword: {text}!");
        
        assembler->SetThreadParameters(threadParameter);
        Console.WriteLine("keyword");
    }
    
    public static void ParseMethodCall(DamnScriptParser.MethodCallContext methodCall, ScriptAssembler* assembler)
    {
        var identifier = methodCall.GetChild(0);
        for (var i = 0; i < methodCall.ChildCount; i++)
        {
            var child = methodCall.GetChild(i);
            Parse(child, assembler);
        }
        
        assembler->NativeCall(identifier.GetText(), methodCall.ChildCount);
    }
    
    public static void ParseIfStatementBody(DamnScriptParser.IfStatementContext ifStatement, ScriptAssembler* assembler)
    {
        Console.WriteLine("ifStatement");
    }
    
    public static void ParseNumberExpression(DamnScriptParser.NumberExpressionContext numberExpression, ScriptAssembler* assembler)
    {
        var number = numberExpression.GetText();
        var value = long.Parse(number);
        assembler->PushToStack(value);
    }

    private static void ParseAdditiveExpression(DamnScriptParser.AdditiveExpressionContext operatorContext, ScriptAssembler* assembler)
    {
        for (var i = 0; i < operatorContext.ChildCount; i++)
        {
            var child = operatorContext.GetChild(i);
            if (child is DamnScriptParser.AdditiveOperatorContext)
                Parse(operatorContext.GetChild(i+=1), assembler);
            Parse(child, assembler);
        }
    }
    
    public static void ParseMultiplicativeOperator(DamnScriptParser.MultiplicativeOperatorContext multiplicativeOperator, ScriptAssembler* assembler)
    {
        var operatorText = multiplicativeOperator.GetText();
        ParseOperator(operatorText, assembler);
    }
    
    public static void ParseAdditiveOperator(DamnScriptParser.AdditiveOperatorContext additiveOperator, ScriptAssembler* assembler)
    {
        var operatorText = additiveOperator.GetText();
        ParseOperator(operatorText, assembler);
    }

    public static void ParseOperator(string operatorText, ScriptAssembler* assembler)
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
            Debugging.LogError($"[{nameof(ScriptParser)}] ({ParseAdditiveOperator}) Unknown operator: {operatorText}!");
        assembler->ExpressionCall(operatorType);
    }
}