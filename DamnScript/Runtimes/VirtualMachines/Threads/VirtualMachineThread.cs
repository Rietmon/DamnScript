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
    
        public ref VirtualMachineThread RefValue => ref UnsafeUtilities.AsRef<VirtualMachineThread>(value);
    
        public VirtualMachineThreadPtr(VirtualMachineThread* value) => this.value = value;

        public static implicit operator VirtualMachineThreadPtr(VirtualMachineThread* value) => new(value);
    
        public static implicit operator VirtualMachineThread*(VirtualMachineThreadPtr ptr) => ptr.value;
    }

    public unsafe partial struct VirtualMachineThread : IDisposable
    {
        private byte* ByteCode => regionData->byteCode.start + offset;

        public VirtualMachineThreadStack stack;
        public VirtualMachineRegisters registers;

        public readonly String32* scriptName;
        public readonly RegionData* regionData;
        public readonly ScriptMetadata* metadata;
        
        public ObjectPin awaitTaskPin;
    
        public int offset;
        public int savePoint;

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
                    offset += NativeCall.size;
                    break;
                }
                case PushToStack.OpCode:
                {
                    ExecutePushToStack(*(PushToStack*)byteCode);
                    offset += PushToStack.size;
                    break;
                }
                case ExpressionCall.OpCode:
                {
                    ExecuteExpressionCall(*(ExpressionCall*)byteCode);
                    offset += ExpressionCall.size;
                    break;
                }
                case SetSavePoint.OpCode:
                {
                    ExecuteSetSavePoint();
                    offset += SetSavePoint.size;
                    break;
                }
                case JumpNotEquals.OpCode:
                {
                    if (ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode))
                        offset += JumpNotEquals.size;
                    break;
                }
                case JumpEquals.OpCode:
                {
                    if (ExecuteJumpIfEquals(*(JumpEquals*)byteCode))
                        offset += JumpEquals.size;
                    break;
                }
                case Jump.OpCode:
                {
                    if (ExecuteJump(*(Jump*)byteCode))
                        offset += Jump.size;
                    break;
                }
                case PushStringToStack.OpCode:
                {
                    ExecutePushStringToStack(*(PushStringToStack*)byteCode);
                    offset += JumpEquals.size;
                    break;
                }
                case SetThreadParameters.OpCode:
                {
                    ExecuteSetThreadParameters(*(SetThreadParameters*)byteCode);
                    offset += SetThreadParameters.size;
                    break;
                }
                case StoreToRegister.OpCode:
                {
                    ExecuteStoreToRegister(*(StoreToRegister*)byteCode);
                    offset += StoreToRegister.size;
                    break;
                }
                case LoadFromRegister.OpCode:
                {
                    ExecuteLoadFromRegister(*(LoadFromRegister*)byteCode);
                    offset += LoadFromRegister.size;
                    break;
                }
                case DuplicateStack.OpCode:
                {
                    ExecuteDuplicateStack(*(DuplicateStack*)byteCode);
                    offset += DuplicateStack.size;
                    break;
                }
                case OpCodes.OpCodes.Invalid:
                default:
                    throw new NotSupportedException($"Invalid OpCode: {opCode}");
            }

            return true;
        }

        public void Dispose()
        {
            isDisposed = true;
        }
    }
}