using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct StoreToRegister
    {
        public const OpCodeType OpCode = OpCodeType.StoreToRegister;
        public static readonly int size = sizeof(StoreToRegister);
        public readonly OpCodeType opCode;
        public readonly int register;

        public StoreToRegister(int register)
        {
            opCode = OpCode;
            this.register = register;
        }
    }
}