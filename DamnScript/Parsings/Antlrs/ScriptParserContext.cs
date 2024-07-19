using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.VirtualMachines.Assemblers;

namespace DamnScript.Parsings.Antlrs
{
    public unsafe struct ScriptParserContext
    {
        public NativeList<UnsafeStringPair>* strings;
    
        public ScriptAssembler* assembler;

        public string loopRegisterIdentifier0;
        public string loopRegisterIdentifier1;
        public string loopRegisterIdentifier2;
        public string loopRegisterIdentifier3;

        public bool isError;
        
        public int ReserveIdentifier(string identifier)
        {
            if (loopRegisterIdentifier0 == null)
            {
                loopRegisterIdentifier0 = identifier;
                return 0;
            }

            if (loopRegisterIdentifier1 == null)
            {
                loopRegisterIdentifier1 = identifier;
                return 1;
            }

            if (loopRegisterIdentifier2 == null)
            {
                loopRegisterIdentifier2 = identifier;
                return 2;
            }

            if (loopRegisterIdentifier3 == null)
            {
                loopRegisterIdentifier3 = identifier;
                return 3;
            }

            return -1;
        }
        
        public int GetRegisterIndex(string identifier)
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
                    loopRegisterIdentifier0 = null;
                    break;
                case 1:
                    loopRegisterIdentifier1 = null;
                    break;
                case 2:
                    loopRegisterIdentifier2 = null;
                    break;
                case 3:
                    loopRegisterIdentifier3 = null;
                    break;
            }
        }
    }
}