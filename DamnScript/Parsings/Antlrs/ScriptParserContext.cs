using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.VirtualMachines.Assemblers;

namespace DamnScript.Parsings.Antlrs
{
    public unsafe struct ScriptParserContext
    {
        public NativeList<UnsafeStringPair>* strings;
    
        public ScriptAssembler* assembler;

        public bool isError;
    }
}