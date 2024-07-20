using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;

namespace DamnScriptTest;

public static class Shared
{
    public static void PrintDisassembly(ScriptDataPtr scriptData)
    {
        var disassembly = ScriptDisassembler.DisassembleToString(scriptData.RefValue.regions[0].byteCode, scriptData.RefValue.metadata);
        Console.WriteLine(disassembly);
    }
}