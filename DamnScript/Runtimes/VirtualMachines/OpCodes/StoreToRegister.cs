using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StoreToRegister
    {
        public const int OpCode = 0xA;
        public readonly int opCode = OpCode;
        public int register;

        public StoreToRegister(int register)
        {
            this.register = register;
        }
    }
}