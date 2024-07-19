using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PeekFromRegister
    {
        public const int OpCode = 0xB;
        public readonly int opCode = OpCode;

        public int register;

        public PeekFromRegister(int register)
        {
            this.register = register;
        }
    }
}