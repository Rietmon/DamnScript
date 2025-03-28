using System;
using System.Text;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.Debugs
{
    public static unsafe class ScriptDisassembler
    {
        public static unsafe string DisassembleScriptToString(ScriptDataPtr scriptData)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"---{scriptData.value->name.ToString()}---");   
            sb.AppendLine("Metadata:");
            sb.AppendLine("           ");
            sb.AppendLine($"Constants:");
            for (var i = 0; i < scriptData.value->metadata.constants.strings.Length; i++)
            {
                var str = scriptData.value->metadata.constants.strings[i].value;
                sb.AppendLine($"{i.ToString()}: {str->ToString()}");
            }
            sb.AppendLine("           ");
            sb.AppendLine($"Methods:");
            for (var i = 0; i < scriptData.value->metadata.constants.methods.Length; i++)
            {
                var str = scriptData.value->metadata.constants.methods[i].value;
                sb.AppendLine($"{i.ToString()}: {str->ToString()}");
            }
            sb.Append('\n');
            sb.AppendLine("Regions:\n");
            for (var i = 0; i < scriptData.value->regions.Length; i++)
            {
                sb.AppendLine(DisassembleRegionToString(scriptData.value->regions.Begin + i, scriptData.value->metadata));
            }

            return sb.ToString();
        }
        
        public static string DisassembleRegionToString(RegionDataPtr region, ScriptMetadata metadata)
        {
            var ptr = region.value;
            var sb = new StringBuilder();
            sb.Append("           ");         
            sb.Append(ptr->name.ToString());
            sb.AppendLine(":");
            
            var offset = 0;
            var data = ptr->byteCode;
            while (data.IsInRange(offset))
            {
                var byteCode = data.start + offset;
                var opCode = *(OpCodeType*)byteCode;
                switch (opCode)
                {
                    case NativeCall.OpCode:
                    {
                        var nativeCall = *(NativeCall*)byteCode;
                        sb.AppendLine(
                            $"{offset.ToString()}: CALL {nativeCall.MethodIndex} " +
                            $"({metadata.GetMethodName(nativeCall.MethodIndex)->ToString32()}) " +
                            $"{nativeCall.ArgumentsCount.ToString()}");
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