using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Threads;

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
    private const int StackSize = 256;

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
    
    public bool ExecuteNext(out IAsyncResult result)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ExecuteNativeCall(NativeCall nativeCall, out IAsyncResult result)
    {
        result = null;
        var methodName = new string(nativeCall.name);
        if (!VirtualMachineData.TryGetNativeMethod(methodName, out var method))
            return false;
        
        var methodPointer = method.methodPointer;
        var argumentsCount = method.argumentsCount;
        var noAwait = _threadParameters & SetThreadParameters.ThreadParameters.NoAwait;
        if (noAwait != 0)
            _threadParameters ^= SetThreadParameters.ThreadParameters.NoAwait;
        
        if (!method.isAsync)
        {
            switch (argumentsCount)
            {
                case 0: ((delegate*<void>)methodPointer)(); break;
                case 1: ((delegate*<ScriptValue, void>)methodPointer)(new ScriptValue(Pop())); break;
                case 2: ((delegate*<ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop()); break;
                case 3: ((delegate*<ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop()); break;
                case 4: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop()); break;
                case 5: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 6: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 7: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 8: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 9: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 10: ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, void>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                default: throw new Exception("Invalid arguments count! It must be between 0 and 10.");
            }
        }
        else
        {
            result = argumentsCount switch
            {
                0 => ((delegate*<IAsyncResult>)methodPointer)(),
                1 => ((delegate*<ScriptValue, IAsyncResult>)methodPointer)(new ScriptValue(Pop())),
                2 => ((delegate*<ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop()),
                3 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop()),
                4 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop()),
                5 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop()),
                6 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop()),
                7 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()),
                8 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()),
                9 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()),
                10 => ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()),
                _ => throw new Exception("Invalid arguments count! It must be between 0 and 10.")
            };

            return false;
        }

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
        
        Push(new ScriptValue(str).longValue);
        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ExecuteSetThreadParameters(SetThreadParameters setThreadParameters)
    {
        _threadParameters = setThreadParameters.parameters;
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(long value) => _stack.Push(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long Pop() => _stack.Pop();

    public void Dispose()
    {
        _isDisposed = true;
    }
}