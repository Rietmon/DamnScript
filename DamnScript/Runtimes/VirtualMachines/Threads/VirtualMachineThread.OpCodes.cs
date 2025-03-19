#define DAMN_SCRIPT_ENABLE_EXECUTION_LOG
using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Datas;
using DamnScript.Runtimes.VirtualMachines.OpCodes;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	public unsafe partial struct VirtualMachineThread
	{
        private static void Print(string message) 
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
            => Debugging.Log(message);
#else
        { }
#endif
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteNativeCall(NativeCall nativeCall)
        {
            Print($"Begin native call...");
            var methodName = metadata->GetMethodName(nativeCall.methodIndex)->ToString32();
            var argumentsCount = nativeCall.argumentsCount;
            if (!VirtualMachineData.TryGetNativeMethod(methodName, argumentsCount, out var method))
                throw new Exception($"Method \"{methodName}\" with {argumentsCount} arguments not found!");
        
            Print($"Found native method \"{methodName}\" with {argumentsCount} arguments.");
            var argumentsStack = stackalloc ScriptValue[method.argumentsCount];
            for (var i = method.argumentsCount - 1; i >= 0; i--)
                argumentsStack[i] = StackPop();
        
            var returnValue = VirtualMachineInvokeHelper.Invoke(method, argumentsStack, out var result);

            if (result != null)
            {
                Print($"Method is async, pin result...");
                awaitTaskPin = UnsafeUtilities.Pin(result);
            }
        
            for (var i = 0; i < method.argumentsCount; i++)
            {
                var argument = argumentsStack[i];
                if (argument.type == ScriptValue.ValueType.ReferenceSafePointer)
                    argument.UnpinManagedPointer();
            }
            Print($"Free arguments stack...");
        
            if (method.hasReturnValue && !method.isAsync)
            {
                Print($"Push return value ({returnValue.type}):({returnValue.longValue}) to stack...");
                StackPush(returnValue);
            }
            Print($"End native call.");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecutePushToStack(PushToStack pushToStack)
        {
            Print($"Push to stack ({pushToStack.value})");
            StackPush(pushToStack.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteExpressionCall(ExpressionCall expressionCall)
        {
            Print($"Begin expression call ({expressionCall.type})...");
            switch (expressionCall.type)
            {
                case ExpressionCall.ExpressionCallType.Add:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left + right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Subtract:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left - right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Multiply:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left * right);
                    break;
                }
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
                case ExpressionCall.ExpressionCallType.Greater:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left > right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.GreaterOrEqual:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left >= right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.Less:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left < right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.LessOrEqual:
                {
                    var right = StackPop();
                    var left = StackPop();
                    StackPush(left <= right);
                    break;
                }
                case ExpressionCall.ExpressionCallType.And: StackPush(StackPop() != 0 && StackPop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Or: StackPush(StackPop() != 0 || StackPop() != 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Not: StackPush(StackPop() == 0 ? 1 : 0); break;
                case ExpressionCall.ExpressionCallType.Test: StackPush(StackPop() != 0 ? 1 : 0); break;
                default: throw new ArgumentOutOfRangeException($"{nameof(expressionCall)} == {expressionCall.type}");
            }
            Print($"End expression call");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteSetSavePoint()
        {
            Print($"Set save point...");
            savePoint = offset;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
        {
            Print($"Begin Jump not equals...");
            if (StackPop() == StackPop()) 
                return false;
        
            Print($"Jump to {offset}");
            offset = jumpNotEquals.jumpOffset;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpIfEquals(JumpEquals jumpEquals)
        {
            Print($"Begin Jump equals...");
            if (StackPop() != StackPop())
                return false;
        
            Print($"Jump to {offset}");
            offset = jumpEquals.jumpOffset;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ExecuteJump(Jump jump)
        {
            Print($"Jump to {offset}");
            offset = jump.jumpOffset;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecutePushStringToStack(PushStringToStack pushStringToStack)
        {
            Print($"Push string to stack...");
            var index = pushStringToStack.index;
            var str = metadata->GetNativeString(index);
            if (str == null)
                throw new Exception($"String not found by index: {index}");
        
            StackPush(str);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteSetThreadParameters(SetThreadParameters setThreadParameters)
        {
            throw new NotImplementedException();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteStoreToRegister(StoreToRegister storeToRegister)
        {
            Print("Begin store to register...");
            var registerIndex = storeToRegister.register;
            Print($"Store to ({registerIndex}) register...");
            registers[registerIndex] = StackPop().longValue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteLoadFromRegister(LoadFromRegister loadFromRegister)
        {
            Print("Begin load from register...");
            var registerIndex = loadFromRegister.register;
            Print($"Load from ({registerIndex}) register...");
            StackPush(registers[registerIndex]);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteDuplicateStack(DuplicateStack _)
        {
            Print("Begin duplicate stack...");
            StackPush(StackPeek());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StackPush(ScriptValue value)
        {
            Print($"STACK: Push value ({value.type}):({value.longValue}) to stack...");
            stack.Push(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPop()
        {
            var value = stack.Pop();
            Print($"STACK: Pop value ({value.type}):({value.longValue}) from stack...");
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPeek()
        {
            var value = stack.Peek();
            Print($"STACK: Peek value ({value.type}):({value.longValue}) from stack...");
            return value;
        }
	}
}