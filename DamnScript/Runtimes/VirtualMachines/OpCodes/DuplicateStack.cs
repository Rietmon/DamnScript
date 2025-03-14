using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DuplicateStack
    {
        public const OpCodes OpCode = OpCodes.DuplicateStack;
        public readonly OpCodes opCode;

        public DuplicateStack(int _)
        {
            opCode = OpCode;
        }
    }
}