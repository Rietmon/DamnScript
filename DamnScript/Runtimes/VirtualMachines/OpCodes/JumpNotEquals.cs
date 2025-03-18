using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct JumpNotEquals
    {
        public const OpCodes OpCode = OpCodes.JumpNotEquals;
        public static readonly int size = sizeof(JumpNotEquals);
        
        public readonly OpCodes opCode;
        public readonly int jumpOffset;
    
        public JumpNotEquals(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }
    }
}