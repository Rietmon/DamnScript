using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct StoreToRegister
    {
        public const OpCodes OpCode = OpCodes.StoreToRegister;
        public readonly OpCodes opCode;
        public int register;

        public StoreToRegister(int register)
        {
            opCode = OpCode;
            this.register = register;
        }
    }
}