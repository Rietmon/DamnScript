using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct PushToStack
    {
        public const OpCodes OpCode = OpCodes.PushToStack;
        public static readonly int size = sizeof(PushToStack);
        
        public readonly OpCodes opCode;
        public readonly long value;

        public PushToStack(long value)
        {
            opCode = OpCode;
            this.value = value;
        }
    }
}