using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Metadatas;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Threads;

public unsafe struct VirtualMachineThread
{
    public const int StackSize = 256;
    
    public byte* ByteCode => regionData->byteCode.start + offset;
    
    public readonly RegionData* regionData;
    public readonly ScriptMetadata* metadata;
    
    public fixed byte stack[StackSize];
    public byte* stackPointer;
    public int offset;
    public int savePoint;

    public VirtualMachineThread(RegionData* regionData, ScriptMetadata* metadata)
    {
        this.regionData = regionData;
        this.metadata = metadata;
        fixed (byte* pStack = stack)
            stackPointer = pStack;
    }
    
    public bool ExecuteNext(out IAsyncResult result)
    {
        result = null;
        if (!regionData->byteCode.IsInRange(offset))
            return false;
        
        var byteCode = ByteCode;
        var opCode = *(int*)byteCode;
        switch (opCode)
        {
            case NativeCall.OpCode:
                ExecuteNativeCall(*(NativeCall*)byteCode, out result);
                offset += sizeof(NativeCall);
                break;
            case PushToStack.OpCode:
                ExecutePushToStack(*(PushToStack*)byteCode);
                offset += sizeof(PushToStack);
                break;
            case ExpressionCall.OpCode:
                ExecuteExpressionCall(*(ExpressionCall*)byteCode);
                offset += sizeof(ExpressionCall);
                break;
            case SetSavePoint.OpCode:
                ExecuteSetSavePoint();
                offset += sizeof(SetSavePoint);
                break;
            case JumpNotEquals.OpCode:
                ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode);
                offset += sizeof(JumpNotEquals);
                break;
            case JumpIfEquals.OpCode:
                ExecuteJumpIfEquals(*(JumpIfEquals*)byteCode);
                offset += sizeof(JumpIfEquals);
                break;
            case PushStringToStack.OpCode:
                ExecutePushStringToStack(*(PushStringToStack*)byteCode);
                offset += sizeof(JumpIfEquals);
                break;
            default:
                Debugging.LogError($"[{nameof(VirtualMachineThread)}] ({nameof(ExecuteNext)}) " +
                                   $"Invalid OpCode: {opCode}");
                return false;
        }

        return true;
    }
    
    public bool ExecuteNativeCall(NativeCall nativeCall, out IAsyncResult result)
    {
        result = null;
        var methodName = new string(nativeCall.name);
        if (!VirtualMachine.TryGetNativeMethod(methodName, out var method))
            return false;
        
        var methodPointer = method.methodPointer;
        var argumentsCount = method.argumentsCount;
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
                default: Debugging.LogError($"[{nameof(VirtualMachineThread)}] ({nameof(ExecuteNativeCall)}) " +
                                            $"Invalid arguments count: {argumentsCount}"); break;
            }
        }
        else
        {
            switch (argumentsCount)
            {
                case 0: result = ((delegate*<IAsyncResult>)methodPointer)(); break;
                case 1: result = ((delegate*<ScriptValue, IAsyncResult>)methodPointer)(new ScriptValue(Pop())); break;
                case 2: result = ((delegate*<ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop()); break;
                case 3: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop()); break;
                case 4: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop()); break;
                case 5: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 6: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 7: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 8: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 9: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                case 10: result = ((delegate*<ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, ScriptValue, IAsyncResult>)methodPointer)(Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop(), Pop()); break;
                default: Debugging.LogError($"[{nameof(VirtualMachineThread)}] ({nameof(ExecuteNativeCall)}) " +
                                            $"Invalid arguments count: {argumentsCount}"); break;
            }

            return false;
        }

        return true;
    }

    public bool ExecutePushToStack(PushToStack pushToStack)
    {
        Push(pushToStack.value);
        return true;
    }

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
            default: throw new ArgumentOutOfRangeException();
        }

        return true;
    }

    public bool ExecuteSetSavePoint()
    {
        savePoint = offset;
        return true;
    }
    
    public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
    {
        if (Pop() != Pop())
            offset += jumpNotEquals.jumpOffset;
        return true;
    }
    
    public bool ExecuteJumpIfEquals(JumpIfEquals jumpIfEquals)
    {
        if (Pop() == Pop())
            offset += jumpIfEquals.jumpOffset;
        return true;
    }
    
    public bool ExecutePushStringToStack(PushStringToStack pushStringToStack)
    {
        var hash = pushStringToStack.hash;
        var str = metadata->GetUnsafeString(hash);
        if (str == null)
        {
            Debugging.LogError($"[{nameof(VirtualMachineThread)}] ({nameof(ExecutePushStringToStack)}) " +
                               $"String not found with hash: {hash}"!);
            return false;
        }
        
        Push(new ScriptValue(str).longValue);
        return true;
    }
    
    public void Push(long value)
    {
        fixed (byte* pStack = stack)
        {
            if (stackPointer == pStack + StackSize)
            {
                Debugging.LogError($"[{nameof(VirtualMachineThread)}] ({nameof(Push)}) " +
                                   $"Stack is full!");
                return;
            }
        }
        *(long*)stackPointer = value;
        stackPointer += sizeof(long);
    }
    
    public long Pop()
    {
        fixed (byte* pStack = stack)
        {
            if (stackPointer == pStack)
            {
                Debugging.LogError($"[{nameof(VirtualMachineThread)}] ({nameof(Pop)}) " +
                                   $"Stack is empty!");
                return 0;
            }
        }
        stackPointer -= sizeof(long);
        return *(long*)stackPointer;
    }
}