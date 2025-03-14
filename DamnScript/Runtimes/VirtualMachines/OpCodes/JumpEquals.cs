using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JumpEquals
    {
        public const OpCodes OpCode = OpCodes.JumpEquals;
        public readonly OpCodes opCode;
        public readonly int jumpOffset;
    
        public JumpEquals(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }
    }
}