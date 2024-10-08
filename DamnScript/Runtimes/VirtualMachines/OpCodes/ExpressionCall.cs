﻿using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.VirtualMachines.OpCodes
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ExpressionCall
    {
        public const int OpCode = 0x3;
        public readonly int opCode;
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