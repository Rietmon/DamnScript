using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PushStringToStack
    {
        public const OpCodes OpCode = OpCodes.PushStringToStack;
        public readonly OpCodes opCode;
        public readonly int index;

        public PushStringToStack(int index)
        {
            opCode = OpCode;
            this.index = index;
        }
    }
}