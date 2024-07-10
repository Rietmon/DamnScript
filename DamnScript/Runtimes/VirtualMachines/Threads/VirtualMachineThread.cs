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
    
        private int _offset;
        private int _savePoint;
    
        private SetThreadParameters.ThreadParameters _threadParameters;

        private bool _isDisposed;

        public VirtualMachineThread(RegionData* regionData, ScriptMetadata* metadata)
        {
            _regionData = regionData;
            _metadata = metadata;
            _stack = new VirtualMachineThreadStack();
        }
    
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
            var methodName = new string(nativeCall.name);
            if (!VirtualMachineData.TryGetNativeMethod(methodName, out var method))
                return false;
        
            var argumentsStack = stackalloc ScriptValue[method.argumentsCount];
            for (var i = method.argumentsCount - 1; i >= 0; i--)
                argumentsStack[i] = Pop();
        
            var returnValue = VirtualMachineInvokeHelper.Invoke(method, argumentsStack, out result);
        
            for (var i = 0; i < method.argumentsCount; i++)
            {
                var argument = argumentsStack[i];
                if (argument.type == ScriptValue.ValueType.ReferenceSafePointer)
                    argument.UnpinManagedPointer();
            }
        
            if (method.hasReturnValue)
                Push(returnValue);
        
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecutePushToStack(PushToStack pushToStack)
        {
            Push(pushToStack.value);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteExpressionCall(ExpressionCall expressionCall)
        {
            switch (expressionCall.type)
            {
                case ExpressionCall.ExpressionCallType.Invalid: break;
                case ExpressionCall.ExpressionCallType.Add: Push(Pop() + Pop()); break;
                case ExpressionCall.ExpressionCallType.Subtract: Push(Pop() - Pop()); break;
                case ExpressionCall.ExpressionCallType.Multiply: Push(Pop() * Pop()); break;
                case ExpressionCall.ExpressionCallType.Divide: Push(Pop() / Pop()); break;
                case ExpressionCall.ExpressionCallType.Modulo: Push(Pop() % Pop()); break;
                case ExpressionCall.ExpressionCallType.Negate: Push(-Pop()); break;
                case ExpressionCall.ExpressionCallType.Equal: Push(Pop() == Pop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.NotEqual: Push(Pop() != Pop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Greater: Push(Pop() > Pop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.GreaterOrEqual: Push(Pop() >= Pop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Less: Push(Pop() < Pop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.LessOrEqual: Push(Pop() <= Pop() ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.And: Push(Pop() != 0 && Pop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Or: Push(Pop() != 0 || Pop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Not: Push(Pop() == 0 ? 1 : 0); break;
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
            if (Pop() == Pop()) 
                return true;
        
            _offset = jumpNotEquals.jumpOffset;
            return false;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpIfEquals(JumpIfEquals jumpIfEquals)
        {
            if (Pop() != Pop())
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
        
            Push(str);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteSetThreadParameters(SetThreadParameters setThreadParameters)
        {
            _threadParameters = setThreadParameters.parameters;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(ScriptValue value) => _stack.Push(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue Pop() => _stack.Pop();

        public void Dispose()
        {
            _isDisposed = true;
        }
    }
}