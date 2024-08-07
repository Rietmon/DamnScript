using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
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