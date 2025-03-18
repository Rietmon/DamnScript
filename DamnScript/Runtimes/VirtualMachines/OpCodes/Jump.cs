using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct Jump
    {
        public const OpCodes OpCode = OpCodes.Jump;
        public static readonly int size = sizeof(Jump);
        
        public readonly OpCodes opCode;
        public readonly int jumpOffset;
    
        public Jump(int jumpOffset)
        {
            opCode = OpCode;
            this.jumpOffset = jumpOffset;
        }
    }
}