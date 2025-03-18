using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SetThreadParameters
    {
        public const OpCodes OpCode = OpCodes.SetThreadParameters;
        public static readonly int size = sizeof(SetThreadParameters);
        
        public readonly OpCodes opCode;
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