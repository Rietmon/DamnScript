using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptExamples.NET
{
    public static class Example3
    {
        private const string Code = @"
        region Main
        {
            Log(2);
            LogWithDelay(GetInt() + 5 * 2));
            Log(""Print has done!"");
        }
";

        // This is an exception to the rule, because we are using async/await
        // Async methods can use Task as a return type ("Task" == "void")
        // If we gonna return any value from async method - it should be "Task<ScriptValuePtr>"
        public static async Task LogWithDelay(ScriptValuePtr value)
        {
            await Task.Delay(1000);
            Console.WriteLine(value.LongValue);
        }
    
        public static ScriptValuePtr GetInt()
        {
            // For built-in types, you can use extension method "Return" and "ReturnAsync"
            return 5.Return();
        }
    
        public static void Run()
        {
            ScriptEngine.RegisterNativeMethod(LogWithDelay);
            ScriptEngine.RegisterNativeMethod(GetInt);
        
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example3");
            Shared.PrintDisassembly(scriptData);
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            while (ScriptEngine.ExecuteVirtualMachineNext())
                Thread.Sleep(15);
            Console.Write("\n");
        
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}