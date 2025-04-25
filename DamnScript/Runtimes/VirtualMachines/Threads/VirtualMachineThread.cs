using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Debugs;
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
        private byte* CurrentOpCode => regionData->byteCode.start + offset;

        public VirtualMachineThreadStack stack;
        public VirtualMachineThreadParametersStack parametersStack;
        public VirtualMachineThreadRegisters threadRegisters;

        public ScriptValue returnValue;

        public readonly ScriptData* scriptData;
        public readonly RegionData* regionData; // Rietmon: TODO: Might change to int index?
        public readonly ScriptMetadata* metadata;
        
        public PinHandle awaitTaskPin;
        
        public VirtualMachineThreadParameters threadParameters;
    
        public int offset;
        public int savePoint;

        public bool isAlive;

        public VirtualMachineThread(ScriptData* scriptData, RegionData* regionData, ScriptMetadata* metadata)
        {
            stack = new VirtualMachineThreadStack();
            parametersStack = new VirtualMachineThreadParametersStack();
            threadRegisters = new VirtualMachineThreadRegisters();
            
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
        
            var opCode = CurrentOpCode;
            var type = *(OpCodeType*)opCode;
            switch (type)
            {
                case NativeCall.OpCode:
                {
                    ExecuteNativeCall(*(NativeCall*)opCode);
                    offset += NativeCall.size;
                    break;
                }
                case PushToStack.OpCode:
                {
                    ExecutePushToStack(*(PushToStack*)opCode);
                    offset += PushToStack.size;
                    break;
                }
                case ExpressionCall.OpCode:
                {
                    ExecuteExpressionCall(*(ExpressionCall*)opCode);
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
                    if (!ExecuteJumpNotEquals(*(JumpNotEquals*)opCode))
                        offset += JumpNotEquals.size;
                    break;
                }
                case JumpEquals.OpCode:
                {
                    if (!ExecuteJumpIfEquals(*(JumpEquals*)opCode))
                        offset += JumpEquals.size;
                    break;
                }
                case Jump.OpCode:
                {
                    if (!ExecuteJump(*(Jump*)opCode))
                        offset += Jump.size;
                    break;
                }
                case PushStringToStack.OpCode:
                {
                    ExecutePushStringToStack(*(PushStringToStack*)opCode);
                    offset += PushStringToStack.size;
                    break;
                }
                case StoreToRegister.OpCode:
                {
                    ExecuteStoreToRegister(*(StoreToRegister*)opCode);
                    offset += StoreToRegister.size;
                    break;
                }
                case LoadFromRegister.OpCode:
                {
                    ExecuteLoadFromRegister(*(LoadFromRegister*)opCode);
                    offset += LoadFromRegister.size;
                    break;
                }
                case DuplicateStack.OpCode:
                {
                    ExecuteDuplicateStack(*(DuplicateStack*)opCode);
                    offset += DuplicateStack.size;
                    break;
                }
                case OpCodeType.Invalid:
                default:
                    throw new NotSupportedException($"Invalid OpCode: {type}");
            }

            return true;
        }
        
        public void UnpinParameters()
        {
            var begin = parametersStack.BeginPtr;
            for (var i = 0; i < VirtualMachineThreadParametersStack.MaxParameters; i++)
            {
                if (begin->type != ScriptValue.ValueType.ReferenceSafePointer)
                    continue;

                begin->UnpinSafePointer();
                begin++;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StackPush(ScriptValue value)
        {
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
            Debugging.Log($"STACK: Push value ({value.type}):({value.rawLong}) to stack...");
#endif
            stack.Push(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPop()
        {
            var value = stack.Pop();
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
            Debugging.Log($"STACK: Pop value ({value.type}):({value.rawLong}) from stack...");
#endif
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPeek()
        {
            var value = stack.Peek();
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
            Debugging.Log($"STACK: Peek value ({value.type}):({value.rawLong}) from stack...");
#endif
            return value;
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