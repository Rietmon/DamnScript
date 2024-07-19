using System.Runtime.InteropServices;
using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;

namespace DamnScriptTest
{
    public static class Test5
    {
        private const string Code = @"
        region Main
        {
            for (i in ""a"") {
                Print(i);
            }
        }
";
    
        public static void Run()
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
            var scriptData = ScriptEngine.LoadScript(memoryStream, "Test5");
            var disassembler = ScriptDisassembler.DisassembleToString(scriptData.RefValue.regions[0].byteCode, scriptData.RefValue.metadata);
            Console.WriteLine(disassembler);
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