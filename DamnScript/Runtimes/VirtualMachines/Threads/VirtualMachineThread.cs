using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.VirtualMachines.OpCodes;
using DamnScript.Runtimes.VirtualMachines.Scripts;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

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

        public VirtualMachineThreadNativeCallInfo nativeCallInfo;
        
        public ScriptValue returnValue;

        public readonly ScriptData* scriptData;
        public readonly RegionData* regionData; // Rietmon: TODO: Might change to int index?
        public readonly ScriptMetadata* metadata;
        
        public PinHandle awaitTaskPin;
        
        public ThreadParameters threadParameters;
    
        public int offset;
        public int savePoint;

        public bool isAlive;

        public VirtualMachineThread(ScriptData* scriptData, RegionData* regionData, ScriptMetadata* metadata)
        {
            stack = new VirtualMachineThreadStack();
            registers = new VirtualMachineRegisters();
            nativeCallInfo = VirtualMachineThreadNativeCallInfo.invalid;
            
            returnValue = new ScriptValue();
            
            this.scriptData = scriptData;
            this.regionData = regionData;
            this.metadata = metadata;
            
            awaitTaskPin = default;
            
            threadParameters = default;
            
            offset = 0;
            savePoint = 0;
            
            isAlive = true;

            scriptData->referencesCount++;
        }
    
        /// <summary>
        /// Begin handle thread. Will work until the end of the bytecode, or until it invokes async method.
        /// </summary>
        /// <returns>Is the end of the bytecode or thread disposed?</returns>
        /// <exception cref="Exception">Invalid opcode</exception>
        public bool ExecuteNextOpCode()
        {
            if (!isAlive)
                return false;
        
            if (!regionData->byteCode.IsInRange(offset))
                return false;
        
            var byteCode = ByteCode;
            var opCode = *(OpCodeType*)byteCode;
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
                    if (!ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode))
                        offset += JumpNotEquals.size;
                    break;
                }
                case JumpEquals.OpCode:
                {
                    if (!ExecuteJumpIfEquals(*(JumpEquals*)byteCode))
                        offset += JumpEquals.size;
                    break;
                }
                case Jump.OpCode:
                {
                    if (!ExecuteJump(*(Jump*)byteCode))
                        offset += Jump.size;
                    break;
                }
                case PushStringToStack.OpCode:
                {
                    ExecutePushStringToStack(*(PushStringToStack*)byteCode);
                    offset += PushStringToStack.size;
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
                case OpCodeType.Invalid:
                default:
                    throw new NotSupportedException($"Invalid OpCode: {opCode}");
            }

            return true;
        }
        
        public void ClearStackAfterNativeCall()
        {
            var begin = stack.Ptr;
            var end = begin - nativeCallInfo.argumentsCount;
            for (var i = begin; i > end; i--)
            {
                var value = stack.Pop();
                if (value.type is not (ScriptValue.ValueType.ReferenceSafePointer or ScriptValue.ValueType.ReferencePersistentSafePointer))
                    continue;

                value.UnpinSafePointer();
            }
        }

        /// <summary>
        /// Mark this thread as dead.
        /// This call will not clear VM thread data, but it should be after the next ExecuteNext call
        /// </summary>
        public void Dispose()
        {
            isAlive = false;
            scriptData->referencesCount--;
        }
    }
}