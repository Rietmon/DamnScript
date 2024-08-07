using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptExamples
{
    public static class Example7
    {
        private const string Code = @"
        region Main
        {
            if (0) {
                Print(""IF"");
            }
            elseif (0) {
                Print(""ELSEIF"");
            }
            else {
                Print(""ELSE"");
            }
        }
";
    
        public static void Run()
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Example7");
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