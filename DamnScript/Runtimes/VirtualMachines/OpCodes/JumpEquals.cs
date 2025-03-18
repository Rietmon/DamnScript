using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct JumpEquals
    {
        public const OpCodes OpCode = OpCodes.JumpEquals;
        public static readonly int size = sizeof(JumpEquals);
        
        public readonly OpCodes opCode;
        public readonly int jumpOffset;
    
        public JumpEquals(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }
    }
}