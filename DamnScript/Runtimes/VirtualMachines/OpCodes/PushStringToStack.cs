using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly struct PushStringToStack
    {
        public const int OpCode = 0x8;

        public readonly int opCode = OpCode;
        public readonly int hash;

        public PushStringToStack(int hash)
        {
            this.hash = hash;
        }
    }
}