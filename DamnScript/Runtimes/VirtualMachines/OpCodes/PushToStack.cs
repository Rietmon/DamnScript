using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PushToStack
    {
        public const int OpCode = 0x2;

        public readonly int opCode;
        public readonly long value;

        public PushToStack(long value)
        {
            opCode = OpCode;
            this.value = value;
        }
    }
}