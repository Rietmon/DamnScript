using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;

namespace DamnScriptTest;

public static class Test1
{
    private const string Code = @"
        region Main
        {
            Print(""Hello from DamnScript!"");
        }
";
    
    public static void Run()
    {
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
        var scriptData = ScriptEngine.LoadScript(memoryStream, "Test1");
        var thread = ScriptEngine.CreateThread(scriptData, "Main");
        ScriptEngine.ExecuteScheduler();
        
        scriptData.RefValue.Dispose();
    }
}