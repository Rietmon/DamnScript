using System;
using System.Text;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.Debugs
{
    public static unsafe class ScriptDisassembler
    {
        public static string DisassembleScriptToString(ScriptDataPtr scriptData)
        {
            var sb = new StringBuilder();
            for (var i = 0; i < scriptData.RefValue.regions.Length; i++)
            {
                var region = scriptData.RefValue.regions[i];
                sb.AppendLine(DisassembleRegionToString(region, scriptData.RefValue.metadata));
            }

            return sb.ToString();
        }
        
        public static string DisassembleRegionToString(RegionData region, ScriptMetadata metadata)
        {
            var sb = new StringBuilder();
            sb.Append("           ");         
            sb.Append(region.name.ToString());
            sb.AppendLine(":");
            
            var offset = 0;
            var data = region.byteCode;
            while (data.IsInRange(offset))
            {
                var byteCode = data.start + offset;
                var opCode = *(OpCodes*)byteCode;
                switch (opCode)
                {
                    case NativeCall.OpCode:
                    {
                        var nativeCall = *(NativeCall*)byteCode;
                        sb.AppendLine(
                            $"{offset.ToString()}: CALL {nativeCall.methodIndex} " +
                            $"({metadata.GetMethodName(nativeCall.methodIndex)->ToString32()}) " +
                            $"{nativeCall.argumentsCount.ToString()}");
                        offset += sizeof(NativeCall);
                        break;
                    }
                    case PushToStack.OpCode:
                    {
                        var pushToStack = *(PushToStack*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: PUSH {pushToStack.value.ToString()}");
                        offset += sizeof(PushToStack);
                        break;
                    }
                    case ExpressionCall.OpCode:
                    {
                        var expressionCall = *(ExpressionCall*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: EXPCALL {expressionCall.type.ToString()}");
                        offset += sizeof(ExpressionCall);
                        break;
                    }
                    case SetSavePoint.OpCode:
                    {
                        sb.AppendLine($"{offset.ToString()}: SAVE");
                        offset += sizeof(SetSavePoint);
                        break;
                    }
                    case JumpNotEquals.OpCode:
                    {
                        var jumpNotEquals = *(JumpNotEquals*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: JNE {jumpNotEquals.jumpOffset.ToString()}");
                        offset += sizeof(JumpNotEquals);
                        break;
                    }
                    case JumpEquals.OpCode:
                    {
                        var jumpIfEquals = *(JumpEquals*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: JEQ {jumpIfEquals.jumpOffset.ToString()}");
                        offset += sizeof(JumpEquals);
                        break;
                    }
                    case Jump.OpCode:
                    {
                        var jump = *(Jump*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: JMP {jump.jumpOffset.ToString()}");
                        offset += sizeof(Jump);
                        break;
                    }
                    case PushStringToStack.OpCode:
                    {
                        var pushStringToStack = *(PushStringToStack*)byteCode;
                        var str = metadata.GetNativeString(pushStringToStack.index);
                        sb.AppendLine(
                            $"{offset.ToString()}: PUSHSTR {pushStringToStack.index.ToString()} ({str->ToString()})");
                        offset += sizeof(JumpEquals);
                        break;
                    }
                    case SetThreadParameters.OpCode:
                    {
                        var setThreadParameters = *(SetThreadParameters*)byteCode;
                        sb.Append($"{offset.ToString()}: STP ");
                        if ((setThreadParameters.parameters & SetThreadParameters.ThreadParameters.NoAwait) != 0)
                            sb.Append("ASYNC");
                        else
                            sb.Append("NOTHING");
                        offset += sizeof(SetThreadParameters);
                        break;
                    }
                    case StoreToRegister.OpCode:
                    {
                        var pushToRegister = *(StoreToRegister*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: STORE {pushToRegister.register.ToString()}");
                        offset += sizeof(StoreToRegister);
                        break;
                    }
                    case LoadFromRegister.OpCode:
                    {
                        var popFromRegister = *(LoadFromRegister*)byteCode;
                        sb.AppendLine($"{offset.ToString()}: LOAD {popFromRegister.register.ToString()}");
                        offset += sizeof(LoadFromRegister);
                        break;
                    }
                    case DuplicateStack.OpCode:
                    {
                        sb.AppendLine($"{offset.ToString()}: DPL");
                        offset += sizeof(DuplicateStack);
                        break;
                    }
                    default:
                        throw new NotSupportedException($"Invalid OpCode: {opCode}");
                }
            }

            return sb.ToString();
        }
    }
}