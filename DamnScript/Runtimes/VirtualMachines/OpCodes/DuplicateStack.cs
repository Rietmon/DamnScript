using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct DuplicateStack
    {
        public const OpCodeType OpCode = OpCodeType.DuplicateStack;
        public static readonly int size = sizeof(DuplicateStack);
        
        public readonly OpCodeType opCode;

        public DuplicateStack(int _)
        {
            opCode = OpCode;
        }
    }
}