using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SetThreadParameters
    {
        public const OpCodeType OpCode = OpCodeType.SetThreadParameters;
        public static readonly int size = sizeof(SetThreadParameters);
        
        public readonly OpCodeType opCode;
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