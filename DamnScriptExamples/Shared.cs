using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Scripts;

namespace DamnScriptExamples
{
    public static class Shared
    {
        public static void PrintDisassembly(ScriptDataPtr scriptData)
        {
            var disassembly = ScriptDisassembler.DisassembleScriptToString(
                scriptData);
            Console.WriteLine(disassembly);
        }
    }
}