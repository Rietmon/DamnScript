using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

namespace DamnScriptExamples
{
    public static class Shared
    {
        public static void PrintDisassembly(ScriptDataPtr scriptData)
        {
            var disassembly = ScriptDisassembler.DisassembleRegionToString(scriptData.RefValue.regions[0], scriptData.RefValue.metadata);
            Console.WriteLine(disassembly);
        }
    }
}