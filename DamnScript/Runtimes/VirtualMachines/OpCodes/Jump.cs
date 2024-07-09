using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Jump
    {
        public const int OpCode = 0x9;
        public readonly int opCode = OpCode;
        public readonly int jumpOffset;
    
        public Jump(int jumpOffset)
        {
            this.jumpOffset = jumpOffset;
        }
    }
}