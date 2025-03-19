using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct LoadFromRegister
    {
        public const OpCodeType OpCode = OpCodeType.LoadFromRegister;
        public static readonly int size = sizeof(LoadFromRegister);
        
        public readonly OpCodeType opCode;
        public readonly int register;

        public LoadFromRegister(int register)
        {
            opCode = OpCode;
            this.register = register;
        }
    }
}