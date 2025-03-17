using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Cores.Types;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
    public readonly unsafe struct VirtualMachineThreadPtr
    {
        public readonly VirtualMachineThread* value; 
    
        public ref VirtualMachineThread RefValue => ref *value;
    
        public VirtualMachineThreadPtr(VirtualMachineThread* value) => this.value = value;

        public static implicit operator VirtualMachineThreadPtr(VirtualMachineThread* value) => new(value);
    
        public static implicit operator VirtualMachineThread*(VirtualMachineThreadPtr ptr) => ptr.value;
    }

    public unsafe partial struct VirtualMachineThread : IDisposable
    {
        private byte* ByteCode => regionData->byteCode.start + offset;

        public readonly String32* scriptName;
        public readonly RegionData* regionData;
        public readonly ScriptMetadata* metadata;

        public VirtualMachineThreadStack stack;
        public VirtualMachineRegisters registers;
    
        public int offset;
        public int savePoint;
        
        public ObjectPin awaitTaskPin;

        public bool isDisposed;

        public VirtualMachineThread(String32* scriptName, RegionData* regionData, ScriptMetadata* metadata)
        {
            this.scriptName = scriptName;
            this.regionData = regionData;
            this.metadata = metadata;
            stack = new VirtualMachineThreadStack();
            registers = new VirtualMachineRegisters();
            offset = 0;
            savePoint = 0;
            awaitTaskPin = default;
            isDisposed = false;
        }
    
        /// <summary>
        /// Begin handle thread. Will work until the end of the bytecode, or until it invokes async method.
        /// </summary>
        /// <returns>Is the end of the bytecode or thread disposed?</returns>
        /// <exception cref="Exception">Invalid opcode</exception>
        public bool ExecuteNext()
        {
            if (isDisposed)
                return false;
        
            if (!regionData->byteCode.IsInRange(offset))
                return false;
        
            var byteCode = ByteCode;
            var opCode = *(OpCodes.OpCodes*)byteCode;
            switch (opCode)
            {
                case NativeCall.OpCode:
                {
                    ExecuteNativeCall(*(NativeCall*)byteCode);
                    offset += sizeof(NativeCall);
                    break;
                }
                case PushToStack.OpCode:
                {
                    ExecutePushToStack(*(PushToStack*)byteCode);
                    offset += sizeof(PushToStack);
                    break;
                }
                case ExpressionCall.OpCode:
                {
                    ExecuteExpressionCall(*(ExpressionCall*)byteCode);
                    offset += sizeof(ExpressionCall);
                    break;
                }
                case SetSavePoint.OpCode:
                {
                    ExecuteSetSavePoint();
                    offset += sizeof(SetSavePoint);
                    break;
                }
                case JumpNotEquals.OpCode:
                {
                    if (ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode))
                        offset += sizeof(JumpNotEquals);
                    break;
                }
                case JumpEquals.OpCode:
                {
                    if (ExecuteJumpIfEquals(*(JumpEquals*)byteCode))
                        offset += sizeof(JumpEquals);
                    break;
                }
                case Jump.OpCode:
                {
                    if (ExecuteJump(*(Jump*)byteCode))
                        offset += sizeof(Jump);
                    break;
                }
                case PushStringToStack.OpCode:
                {
                    ExecutePushStringToStack(*(PushStringToStack*)byteCode);
                    offset += sizeof(JumpEquals);
                    break;
                }
                case SetThreadParameters.OpCode:
                {
                    ExecuteSetThreadParameters(*(SetThreadParameters*)byteCode);
                    offset += sizeof(SetThreadParameters);
                    break;
                }
                case StoreToRegister.OpCode:
                {
                    ExecuteStoreToRegister(*(StoreToRegister*)byteCode);
                    offset += sizeof(StoreToRegister);
                    break;
                }
                case LoadFromRegister.OpCode:
                {
                    ExecuteLoadFromRegister(*(LoadFromRegister*)byteCode);
                    offset += sizeof(LoadFromRegister);
                    break;
                }
                case DuplicateStack.OpCode:
                {
                    ExecuteDuplicateStack(*(DuplicateStack*)byteCode);
                    offset += sizeof(DuplicateStack);
                    break;
                }
                case OpCodes.OpCodes.Invalid:
                default:
                {
                    throw new NotSupportedException($"Invalid OpCode: {opCode}");
                }
            }

            return true;
        }

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}