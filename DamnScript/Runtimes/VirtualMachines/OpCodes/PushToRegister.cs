using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct PushToRegister
    {
        public const int OpCode = 0xA;
        public readonly int opCode = OpCode;
        public int register;

        public PushToRegister(int register)
        {
            this.register = register;
        }
    }
}