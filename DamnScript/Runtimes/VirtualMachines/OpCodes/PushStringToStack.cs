using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PushStringToStack
    {
        public const int OpCode = 0x8;

        public readonly int opCode;
        public readonly int index;

        public PushStringToStack(int index)
        {
            opCode = OpCode;
            this.index = index;
        }
    }
}