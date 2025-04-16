using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptExamples
{
    public static class Example6
    {
        private const string Code = @"
        region Main
        {
            while (CanHandle()) {
                Print(GetCounter());
            }
        }
";

        private static int _counter;
        public static ScriptValuePtr CanHandle() => (++_counter < 10).Return();
        public static ScriptValuePtr GetCounter() => _counter.Return();
    
        public static void Run()
        {
            ScriptEngine.RegisterNativeMethod(CanHandle);
            ScriptEngine.RegisterNativeMethod(GetCounter);
            
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example6");
            Shared.PrintDisassembly(scriptData);
            var thread = ScriptEngine.RunThread(scriptData, "Main");
            _counter = 0;
        
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