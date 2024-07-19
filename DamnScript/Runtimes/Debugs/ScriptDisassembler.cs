﻿using System;
using System.Text;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.Debugs
{
    public static unsafe class ScriptDisassembler
    {
        public static string DisassembleToString(ByteCodeData data, ScriptMetadata metadata)
        {
            var sb = new StringBuilder();
            var offset = 0;
            while (data.IsInRange(offset))
            {
                var byteCode = data.start + offset;
                var opCode = *(int*)byteCode;
                switch (opCode)
                {
                    case NativeCall.OpCode:
                        var nativeCall = *(NativeCall*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: CALL {new string(nativeCall.name)} {nativeCall.argumentsCount.ToString()}");
                        offset += sizeof(NativeCall);
                        break;
                    case PushToStack.OpCode:
                        var pushToStack = *(PushToStack*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: PUSH {pushToStack.value.ToString()}");
                        offset += sizeof(PushToStack);
                        break;
                    case ExpressionCall.OpCode:
                        var expressionCall = *(ExpressionCall*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: EXPCALL {expressionCall.type.ToString()}");
                        offset += sizeof(ExpressionCall);
                        break;
                    case SetSavePoint.OpCode:
                        sb.AppendLine($"{offset.ToString()}: SAVE");
                        offset += sizeof(SetSavePoint);
                        break;
                    case JumpNotEquals.OpCode:
                        var jumpNotEquals = *(JumpNotEquals*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: JNE {jumpNotEquals.jumpOffset.ToString()}");
                        offset += sizeof(JumpNotEquals);
                        break;
                    case JumpIfEquals.OpCode:
                        var jumpIfEquals = *(JumpIfEquals*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: JEQ {jumpIfEquals.jumpOffset.ToString()}");
                        offset += sizeof(JumpIfEquals);
                        break;
                    case Jump.OpCode:
                        var jump = *(Jump*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: JMP {jump.jumpOffset.ToString()}");
                        offset += sizeof(Jump);
                        break;
                    case PushStringToStack.OpCode:
                        var pushStringToStack = *(PushStringToStack*)byteCode;
                        var str = metadata.GetUnsafeString(pushStringToStack.hash);
                        sb.AppendLine($"{offset.ToString()}: PUSHSTR {str->ToString()} ({pushStringToStack.hash.ToString()})");
                        offset += sizeof(JumpIfEquals);
                        break;
                    case SetThreadParameters.OpCode:
                        // Rietmon: Not implemented
                        offset += sizeof(SetThreadParameters);
                        break;
                    case PushToRegister.OpCode:
                        var pushToRegister = *(PushToRegister*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: PUSHREG {pushToRegister.register.ToString()}");
                        offset += sizeof(PushToRegister);
                        break;
                    case PeekFromRegister.OpCode:
                        var popFromRegister = *(PeekFromRegister*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: PEEKREG {popFromRegister.register.ToString()}");
                        offset += sizeof(PeekFromRegister);
                        break;
                    case DuplicateStack.OpCode:
                        sb.AppendLine($"{offset.ToString()}: DPL");
                        offset += sizeof(DuplicateStack);
                        break;
                    default:
                        throw new Exception($"Invalid OpCode: {opCode}");
                }
            }

            return sb.ToString();
        }
    }
}