using System;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    public struct DuplicateStack
    {
        public const int OpCode = 0xC;
    
        public readonly int opCode;

        public DuplicateStack(int _)
        {
            opCode = OpCode;
        }
    }
}