using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Arrays;
using DamnScript.Runtimes.Cores.Strings;
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