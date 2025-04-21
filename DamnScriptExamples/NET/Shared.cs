using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Scripts;

namespace DamnScriptExamples.NET
{
    public static class Shared
    {
        public static void PrintDisassembly(ScriptDataPtr scriptData)
        {
            var disassembly = ScriptDisassembler.DisassembleScriptToString(
                scriptData);
            Debugging.Log(disassembly);
        }
    }
}