using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
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