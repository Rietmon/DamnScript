using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct NativeCall
    {
        public const OpCodeType OpCode = OpCodeType.NativeCall;
        public static readonly int size = sizeof(NativeCall);

        public int MethodIndex => methodIndexAndArgumentsCount & 0xFFFFFF;
        public int ArgumentsCount => (methodIndexAndArgumentsCount >> 24) & 0xFFFFFF;
        
        public readonly OpCodeType opCode;
        public readonly int methodIndexAndArgumentsCount; // Rietmon: First 24 bits are method index, last 8 bits are arguments count

        public NativeCall(int methodIndex, int argumentsCount)
        {
            opCode = OpCode;
            methodIndexAndArgumentsCount = (methodIndex & 0xFFFFFF) | ((argumentsCount & 0xFFFFFF) << 24);
        }
    }
}
