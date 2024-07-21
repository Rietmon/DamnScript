using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LoadFromRegister
    {
        public const int OpCode = 0xB;
        public readonly int opCode;

        public int register;

        public LoadFromRegister(int register)
        {
            opCode = OpCode;
            this.register = register;
        }
    }
}