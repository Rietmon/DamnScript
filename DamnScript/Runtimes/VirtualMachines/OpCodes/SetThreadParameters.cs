using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SetThreadParameters
    {
        public const int OpCode = 0x7;
        public readonly int opCode;
        public ThreadParameters parameters;

        public SetThreadParameters(ThreadParameters parameters)
        {
            opCode = OpCode;
            this.parameters = parameters;
        }

        [Flags]
        public enum ThreadParameters
        {
            None = 0,
            NoAwait = 0x1,
        }
    }
}