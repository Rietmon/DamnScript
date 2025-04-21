using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct PushToStack : IOpCode
    {
        public const OpCodeType OpCode = OpCodeType.PushToStack;
        public static readonly int size = sizeof(PushToStack);
        
        public readonly OpCodeType opCode;
        public readonly long value;

        public PushToStack(long value)
        {
            opCode = OpCode;
            this.value = value;
        }

        public int CalculateHash()
        {
            return opCode.GetHashCode();
        }
        
        public string GetAssemblerDebugInfo()
        {
            return value.ToString();
        }
    }
}