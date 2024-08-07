using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptExamples
{
    public static class Example3
    {
        private const string Code = @"
        region Main
        {
            Print(""Now should be a print with the delay..."");
            PrintWithDelay(GetInt() + 5 * 2));
            Print(""Print has done!"");
        }
";

        // This is an exception to the rule, because we are using async/await
        // Async methods can use Task as a return type ("Task" == "void")
        // If we gonna return any value from async method - it should be "Task<ScriptValue>"
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
            ScriptEngine.RegisterNativeMethod(PrintWithDelay);
            ScriptEngine.RegisterNativeMethod(GetInt);
        
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example3");
            Shared.PrintDisassembly(scriptData);
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            while (ScriptEngine.ExecuteScheduler())
                Thread.Sleep(15);
            Console.Write("\n");
        
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}