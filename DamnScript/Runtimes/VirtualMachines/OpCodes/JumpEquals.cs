using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct JumpEquals
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
    }
}