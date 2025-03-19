using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct NativeCall
    {
        public const OpCodeType OpCode = OpCodeType.NativeCall;
        public static readonly int size = sizeof(NativeCall);
        
        public readonly OpCodeType opCode;
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
