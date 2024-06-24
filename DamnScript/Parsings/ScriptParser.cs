using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using DamnScript.Parsings.G4;
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
        for (var i = 0; i < file.ChildCount - 1; i++)
        {
            var region = ParseRegion(file.GetChild(i));
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
                break;
            case DamnScriptParser.IfStatementContext ifStatement:
                ParseIfStatementBody(ifStatement, assembler);
                break;
            case DamnScriptParser.ArgContext arg:
                ParseArgument(arg, assembler);
                break;
            case DamnScriptParser.ExpressionContext expression:
                ParseExpression(expression, assembler);
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
        Console.WriteLine("methodCall");
    }
    
    public static void ParseIfStatementBody(DamnScriptParser.IfStatementContext ifStatement, ScriptAssembler* assembler)
    {
        Console.WriteLine("ifStatement");
    }

    private static void ParseArgument(DamnScriptParser.ArgContext argContext, ScriptAssembler* assembler)
    {
        Console.WriteLine("arg");
    }
    
    public static void ParseExpression(DamnScriptParser.ExpressionContext expression, ScriptAssembler* assembler)
    {
        Console.WriteLine("expression");
    }
}