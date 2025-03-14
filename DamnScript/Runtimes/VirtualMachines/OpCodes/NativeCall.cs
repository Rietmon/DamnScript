using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeCall
    {
        public const OpCodes OpCode = OpCodes.NativeCall;
        public readonly OpCodes opCode;
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
