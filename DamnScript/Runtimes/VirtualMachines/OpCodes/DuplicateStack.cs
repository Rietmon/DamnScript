namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    public struct DuplicateStack
    {
        public const int OpCode = 0xC;
    
        public readonly int opCode;
        
#if !DAMN_SCRIPT_UNITY
        public DuplicateStack() => throw new Exception("Don't use default constructor.");
#endif

        public DuplicateStack(int _)
        {
            opCode = OpCode;
        }
    }
}