using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.VirtualMachines.Assemblers;

namespace DamnScript.Parsings;

public unsafe struct ScriptParserContext
{
    public NativeList<UnsafeStringPair>* strings;
    
    public ScriptAssembler* assembler;
}