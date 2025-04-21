using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct JumpEquals : IOpCode
    {
        public const OpCodeType OpCode = OpCodeType.JumpEquals;
        public static readonly int size = sizeof(JumpEquals);
        
        public readonly OpCodeType opCode;
        public readonly int jumpOffset;
    
        public JumpEquals(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }

        public int CalculateHash()
        {
            return HashCode.Combine(opCode, jumpOffset);
        }

        public string GetAssemblerDebugInfo()
        {
            return jumpOffset.ToString();
        }
    }
}
