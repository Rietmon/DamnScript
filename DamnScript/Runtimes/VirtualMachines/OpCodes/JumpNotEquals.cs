using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct JumpNotEquals
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
    }
}