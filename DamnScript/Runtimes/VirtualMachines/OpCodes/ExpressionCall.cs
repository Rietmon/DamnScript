using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public readonly unsafe struct ExpressionCall
    {
        public const OpCodes OpCode = OpCodes.ExpressionCall;
        public static readonly int size = sizeof(ExpressionCall);
        
        public readonly OpCodes opCode;
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