using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct JumpNotEquals : IOpCode
    {
        public const OpCodeType OpCode = OpCodeType.JumpNotEquals;
        public static readonly int size = sizeof(JumpNotEquals);
        
        public readonly OpCodeType opCode;
        public readonly int jumpOffset;
    
        public JumpNotEquals(int jumpOffset)
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
