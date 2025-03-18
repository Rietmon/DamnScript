using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct PushStringToStack
    {
        public const OpCodes OpCode = OpCodes.PushStringToStack;
        public static readonly int size = sizeof(PushStringToStack);
        
        public readonly OpCodes opCode;
        public readonly int index;

        public PushStringToStack(int index)
        {
            opCode = OpCode;
            this.index = index;
        }
    }
}