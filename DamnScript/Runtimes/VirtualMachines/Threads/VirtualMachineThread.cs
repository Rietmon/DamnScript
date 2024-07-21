using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
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

    public unsafe struct VirtualMachineThread : IDisposable
    {
        private byte* ByteCode => _regionData->byteCode.start + _offset;

        private readonly RegionData* _regionData;
        private readonly ScriptMetadata* _metadata;

        private VirtualMachineThreadStack _stack;
        private VirtualMachineRegisters _registers;
    
        private int _offset;
        private int _savePoint;
    
        private SetThreadParameters.ThreadParameters _threadParameters;

        private bool _isDisposed;

        public VirtualMachineThread(RegionData* regionData, ScriptMetadata* metadata) : this()
        {
            _regionData = regionData;
            _metadata = metadata;
            _stack = new VirtualMachineThreadStack();
            _registers = new VirtualMachineRegisters();
        }
    
        /// <summary>
        /// Begin handle thread. Will work until the end of the bytecode, or until it invokes async method.
        /// In the second case, it will return "out Task".
        /// </summary>
        /// <param name="result">Is invoked async method?</param>
        /// <returns>Is the end of the bytecode or thread disposed?</returns>
        /// <exception cref="Exception">Invalid opcode</exception>
        public bool ExecuteNext(out Task result)
        {
            result = null;
        
            if (_isDisposed)
                return false;
        
            if (!_regionData->byteCode.IsInRange(_offset))
                return false;
        
            var byteCode = ByteCode;
            var opCode = *(int*)byteCode;
            switch (opCode)
            {
                case NativeCall.OpCode:
                    ExecuteNativeCall(*(NativeCall*)byteCode, out result);
                    _offset += sizeof(NativeCall);
                    break;
                case PushToStack.OpCode:
                    ExecutePushToStack(*(PushToStack*)byteCode);
                    _offset += sizeof(PushToStack);
                    break;
                case ExpressionCall.OpCode:
                    ExecuteExpressionCall(*(ExpressionCall*)byteCode);
                    _offset += sizeof(ExpressionCall);
                    break;
                case SetSavePoint.OpCode:
                    ExecuteSetSavePoint();
                    _offset += sizeof(SetSavePoint);
                    break;
                case JumpNotEquals.OpCode:
                    if (ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode))
                        _offset += sizeof(JumpNotEquals);
                    break;
                case JumpIfEquals.OpCode:
                    if (ExecuteJumpIfEquals(*(JumpIfEquals*)byteCode))
                        _offset += sizeof(JumpIfEquals);
                    break;
                case Jump.OpCode:
                    if (ExecuteJump(*(Jump*)byteCode))
                        _offset += sizeof(Jump);
                    break;
                case PushStringToStack.OpCode:
                    ExecutePushStringToStack(*(PushStringToStack*)byteCode);
                    _offset += sizeof(JumpIfEquals);
                    break;
                case SetThreadParameters.OpCode:
                    ExecuteSetThreadParameters(*(SetThreadParameters*)byteCode);
                    _offset += sizeof(SetThreadParameters);
                    break;
                case StoreToRegister.OpCode:
                    ExecuteStoreToRegister(*(StoreToRegister*)byteCode);
                    _offset += sizeof(StoreToRegister);
                    break;
                case LoadFromRegister.OpCode:
                    ExecuteLoadFromRegister(*(LoadFromRegister*)byteCode);
                    _offset += sizeof(LoadFromRegister);
                    break;
                case DuplicateStack.OpCode:
                    ExecuteDuplicateStack(*(DuplicateStack*)byteCode);
                    _offset += sizeof(DuplicateStack);
                    break;
                default:
                    throw new Exception($"Invalid OpCode: {opCode}");
            }

            return true;
        }

        // Rietmon: TODO: Remove String and use StringHash
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteNativeCall(NativeCall nativeCall, out Task result)
        {
            result = null;
            var methodName = nativeCall.name;
            if (!VirtualMachineData.TryGetNativeMethod(methodName, out var method))
                return false;
        
            var argumentsStack = stackalloc ScriptValue[method.argumentsCount];
            for (var i = method.argumentsCount - 1; i >= 0; i--)
                argumentsStack[i] = StackPop();
        
            var returnValue = VirtualMachineInvokeHelper.Invoke(method, argumentsStack, out result);
        
            for (var i = 0; i < method.argumentsCount; i++)
            {
                var argument = argumentsStack[i];
                if (argument.type == ScriptValue.ValueType.ReferenceSafePointer)
                    argument.UnpinManagedPointer();
            }
        
            if (method.hasReturnValue)
                StackPush(returnValue);
        
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecutePushToStack(PushToStack pushToStack)
        {
            StackPush(pushToStack.value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteExpressionCall(ExpressionCall expressionCall)
        {
            switch (expressionCall.type)
            {
                case ExpressionCall.ExpressionCallType.Invalid: break;
                case ExpressionCall.ExpressionCallType.Add: StackPush(StackPop() + StackPop()); break;
                case ExpressionCall.ExpressionCallType.Subtract:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left - right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Multiply: StackPush(StackPop() * StackPop()); break;
                case ExpressionCall.ExpressionCallType.Divide:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left / right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Modulo:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left % right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Negate: StackPush(-StackPop()); break;
                case ExpressionCall.ExpressionCallType.Equal: StackPush(StackPop() == StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.NotEqual: StackPush(StackPop() != StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Greater: StackPush(StackPop() > StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.GreaterOrEqual: StackPush(StackPop() >= StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Less: StackPush(StackPop() < StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.LessOrEqual: StackPush(StackPop() <= StackPop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.And: StackPush(StackPop() != 0 && StackPop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Or: StackPush(StackPop() != 0 || StackPop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Not: StackPush(StackPop() == 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Test: StackPush(StackPop() != 0 ? 1 : 0); break;
                default: throw new ArgumentOutOfRangeException(nameof(expressionCall));
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteSetSavePoint()
        {
            _savePoint = _offset;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
        {
            if (StackPop() == StackPop()) 
                return true;
        
            _offset = jumpNotEquals.jumpOffset;
            return false;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpIfEquals(JumpIfEquals jumpIfEquals)
        {
            if (StackPop() != StackPop())
                return true;
        
            _offset = jumpIfEquals.jumpOffset;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ExecuteJump(Jump jump)
        {
            _offset = jump.jumpOffset;
            return false;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecutePushStringToStack(PushStringToStack pushStringToStack)
        {
            var hash = pushStringToStack.hash;
            var str = _metadata->GetUnsafeString(hash);
            if (str == null)
                throw new Exception($"String not found with hash: {hash}");
        
            StackPush(str);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteSetThreadParameters(SetThreadParameters setThreadParameters)
        {
            _threadParameters = setThreadParameters.parameters;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteStoreToRegister(StoreToRegister storeToRegister)
        {
            switch (storeToRegister.register)
            {
                case 0: _registers[0] = StackPop().longValue; break;
                case 1: _registers[1] = StackPop().longValue; break;
                case 2: _registers[2] = StackPop().longValue; break;
                case 3: _registers[3] = StackPop().longValue; break;
                default: throw new ArgumentOutOfRangeException(nameof(storeToRegister.register));
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteLoadFromRegister(LoadFromRegister loadFromRegister)
        {
            switch (loadFromRegister.register)
            {
                case 0: StackPush(_registers[0]); break;
                case 1: StackPush(_registers[1]); break;
                case 2: StackPush(_registers[2]); break;
                case 3: StackPush(_registers[3]); break;
                default: throw new ArgumentOutOfRangeException(nameof(loadFromRegister.register));
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteDuplicateStack(DuplicateStack duplicateStack)
        {
            var value = StackPop();
            StackPush(value);
            StackPush(value);

            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StackPush(ScriptValue value) => _stack.Push(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPop() => _stack.Pop();

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}