using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.VirtualMachines.Assemblers;

namespace DamnScript.Parsings.Antlrs
{
    public unsafe struct ScriptParserContext
    {
        public String32 name;
        
        public NativeList<UnsafeStringPair>* strings;
        public ScriptAssembler* assembler;

        public String32 loopRegisterIdentifier0;
        public String32 loopRegisterIdentifier1;
        public String32 loopRegisterIdentifier2;
        public String32 loopRegisterIdentifier3;

        public bool isError;
        
        public int ReserveIdentifier(String32 identifier)
        {
            if (loopRegisterIdentifier0 == default)
            {
                loopRegisterIdentifier0 = identifier;
                return 0;
            }

            if (loopRegisterIdentifier1 == default)
            {
                loopRegisterIdentifier1 = identifier;
                return 1;
            }

            if (loopRegisterIdentifier2 == default)
            {
                loopRegisterIdentifier2 = identifier;
                return 2;
            }

            if (loopRegisterIdentifier3 == default)
            {
                loopRegisterIdentifier3 = identifier;
                return 3;
            }

            return -1;
        }
        
        public int GetRegisterIndex(String32 identifier)
        {
            if (loopRegisterIdentifier0 == identifier)
                return 0;
            if (loopRegisterIdentifier1 == identifier)
                return 1;
            if (loopRegisterIdentifier2 == identifier)
                return 2;
            if (loopRegisterIdentifier3 == identifier)
                return 3;

            return -1;
        }
        
        public void FreeRegister(int index)
        {
            switch (index)
            {
                case 0:
                    loopRegisterIdentifier0 = default;
                    break;
                case 1:
                    loopRegisterIdentifier1 = default;
                    break;
                case 2:
                    loopRegisterIdentifier2 = default;
                    break;
                case 3:
                    loopRegisterIdentifier3 = default;
                    break;
            }
        }
    }
}