using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptTest
{
    public static class Test2
    {
        private const string Code = @"
        region Main
        {
            Print(GetString());
        }
";

        public static ScriptValue GetString()
        {
            return ScriptValue.FromReferencePin("Hello from C#!");
        }
    
        public static void Run()
        {
            ScriptEngine.RegisterNativeMethod(GetString);
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Test2");
            var thread = ScriptEngine.RunThread(scriptData, "Main");
        
            Console.Write("\n");
            ScriptEngine.ExecuteScheduler();
            Console.Write("\n");
        
            ScriptEngine.UnloadScript(scriptData);
        
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}