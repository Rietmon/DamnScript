using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Threads;

public unsafe struct VirtualMachineThread
{
    public byte* byteCode;
    public readonly byte* byteCodeEnd;
    public int savePoint;
    
    public fixed byte stack[2048];
    public byte* stackPointer;
    
    public VirtualMachineThread(byte* byteCode, int length)
    {
        this.byteCode = byteCode;
        byteCodeEnd = byteCode + length;
        fixed (byte* pStack = stack)
            stackPointer = pStack;
    }
    
    public bool ExecuteNext(out IAsyncResult result)
    {
        result = null;
        if (byteCode >= byteCodeEnd)
            return false;
        
        var opCode = *(int*)byteCode;
        switch (opCode)
        {
            case NativeCall.OpCode:
                ExecuteNativeCall(*(NativeCall*)byteCode, out result);
                byteCode += sizeof(NativeCall);
                break;
            case PushToStack.OpCode:
                ExecutePushToStack(*(PushToStack*)byteCode);
                byteCode += sizeof(PushToStack);
                break;
            case ExpressionCall.OpCode:
                ExecuteExpressionCall(*(ExpressionCall*)byteCode);
                byteCode += sizeof(ExpressionCall);
                break;
            case SetSavePoint.OpCode:
                ExecuteSetSavePoint();
                byteCode += sizeof(SetSavePoint);
                break;
            case JumpNotEquals.OpCode:
                ExecuteJumpNotEquals(*(JumpNotEquals*)byteCode);
                byteCode += sizeof(JumpNotEquals);
                break;
            case JumpIfEquals.OpCode:
                ExecuteJumpIfEquals(*(JumpIfEquals*)byteCode);
                byteCode += sizeof(JumpIfEquals);
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
        savePoint = (int)(byteCodeEnd - byteCode);
        return true;
    }
    
    public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
    {
        if (Pop() != Pop())
            byteCode += jumpNotEquals.jumpOffset;
        return true;
    }
    
    public bool ExecuteJumpIfEquals(JumpIfEquals jumpIfEquals)
    {
        if (Pop() == Pop())
            byteCode += jumpIfEquals.jumpOffset;
        return true;
    }
    
    public void Push(long value)
    {
        *(long*)stackPointer = value;
        stackPointer += sizeof(long);
    }
    
    public long Pop()
    {
        stackPointer -= sizeof(long);
        return *(long*)stackPointer;
    }
}