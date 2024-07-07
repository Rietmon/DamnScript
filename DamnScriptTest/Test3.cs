using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptTest;

public static class Test3
{
    private const string Code = @"
        region Main
        {
            Print(""Now should be a print with the delay..."");
            PrintWithDelay(GetInt() + 5 * 2));
            Print(""Print has done!"");
        }
";

    public static async Task PrintWithDelay(ScriptValue value)
    {
        await Task.Delay(1000);
        Console.WriteLine(value.longValue);
    }
    
    public static ScriptValue GetInt()
    {
        return 5;
    }
    
    public static void Run()
    {
        VirtualMachineData.RegisterNativeMethod(PrintWithDelay);
        VirtualMachineData.RegisterNativeMethod(GetInt);
        
        var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
        var scriptData = ScriptEngine.LoadScript(memoryStream, "Test3");
        var thread = ScriptEngine.CreateThread(scriptData, "Main");
        
        Console.Write("\n");
        while (ScriptEngine.ExecuteScheduler())
            Thread.Sleep(15);
        Console.Write("\n");
        
        //ScriptEngine.UnloadScript(scriptData);
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}