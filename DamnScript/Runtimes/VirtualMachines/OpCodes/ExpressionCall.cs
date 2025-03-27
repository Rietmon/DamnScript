using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_OPCODES
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else 
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct ExpressionCall
    {
        public const OpCodeType OpCode = OpCodeType.ExpressionCall;
        public static readonly int size = sizeof(ExpressionCall);
        
        public readonly OpCodeType opCode;
        public readonly ExpressionCallType type;

        public ExpressionCall(ExpressionCallType type)
        {
            opCode = OpCode;
            this.type = type;
        }

        public enum ExpressionCallType
        {
            Invalid,
            Add,
            Subtract,
            Multiply,
            Divide,
            Modulo,
            Negate,
            Equal,
            NotEqual,
            Greater,
            GreaterOrEqual,
            Less,
            LessOrEqual,
            And,
            Or,
            Not,
            Test
        }
    }
}