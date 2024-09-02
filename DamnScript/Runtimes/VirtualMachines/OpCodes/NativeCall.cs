using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeCall
    {
        public const int OpCode = 0x1;
        public readonly int opCode;
        public readonly int methodIndex;
        public readonly int argumentsCount;

        public NativeCall(int methodIndex, int argumentsCount)
        {
            opCode = OpCode;
            this.methodIndex = methodIndex;
            this.argumentsCount = argumentsCount;
        }
    }
}
