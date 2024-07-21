using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JumpNotEquals
    {
        public const int OpCode = 0x5;
        public readonly int opCode;
        public readonly int jumpOffset;
    
        public JumpNotEquals(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }
    }
}