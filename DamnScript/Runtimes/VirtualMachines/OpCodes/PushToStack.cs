using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct PushToStack
    {
        public const OpCodeType OpCode = OpCodeType.PushToStack;
        public static readonly int size = sizeof(PushToStack);
        
        public readonly OpCodeType opCode;
        public readonly long value;

        public PushToStack(long value)
        {
            opCode = OpCode;
            this.value = value;
        }
    }
}