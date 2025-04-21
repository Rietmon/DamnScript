using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct NativeCall : IOpCode
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
        
        public int CalculateHash()
        {
            return opCode.GetHashCode() + methodIndexAndArgumentsCount;
        }
        
        public string GetAssemblerDebugInfo()
        {
            return $"{MethodIndex} {ArgumentsCount}";
        }
    }
}
