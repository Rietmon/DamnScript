using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LoadFromRegister
    {
        public const OpCodes OpCode = OpCodes.LoadFromRegister;
        public readonly OpCodes opCode;
        public int register;

        public LoadFromRegister(int register)
        {
            opCode = OpCode;
            this.register = register;
        }
    }
}