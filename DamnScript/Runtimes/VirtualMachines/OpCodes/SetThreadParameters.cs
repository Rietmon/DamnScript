using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public unsafe struct SetThreadParameters : IOpCode
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

        public int CalculateHash()
        {
            return opCode.GetHashCode() + parameters.GetHashCode();
        }
    }
}