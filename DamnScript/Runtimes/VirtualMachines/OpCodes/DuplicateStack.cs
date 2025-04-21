using System;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct DuplicateStack : IOpCode
    {
        public const OpCodeType OpCode = OpCodeType.DuplicateStack;
        public static readonly int size = sizeof(DuplicateStack);
        
        public readonly OpCodeType opCode;

        public DuplicateStack(int _)
        {
            opCode = OpCode;
        }

        public int CalculateHash()
        {
            return opCode.GetHashCode();
        }

        public string GetAssemblerDebugInfo()
        {
            return string.Empty;
        }
    }
}