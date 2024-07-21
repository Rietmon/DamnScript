using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct JumpIfEquals
    {
        public const int OpCode = 0x6;
        public readonly int opCode;
        public readonly int jumpOffset;
    
        public JumpIfEquals(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }
    }
}