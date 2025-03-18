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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteNativeCall(NativeCall nativeCall)
        {
            var methodName = metadata->GetMethodName(nativeCall.methodIndex)->ToString32();
            var argumentsCount = nativeCall.argumentsCount;
            if (!VirtualMachineData.TryGetNativeMethod(methodName, argumentsCount, out var method))
                throw new Exception($"Method \"{methodName}\" with {argumentsCount} arguments not found!");
        
            var argumentsStack = stackalloc ScriptValue[method.argumentsCount];
            for (var i = method.argumentsCount - 1; i >= 0; i--)
                argumentsStack[i] = StackPop();
        
            var returnValue = VirtualMachineInvokeHelper.Invoke(method, argumentsStack, out var result);

            if (result != null)
                awaitTaskPin = UnsafeUtilities.Pin(result);
        
            for (var i = 0; i < method.argumentsCount; i++)
            {
                var argument = argumentsStack[i];
                if (argument.type == ScriptValue.ValueType.ReferenceSafePointer)
                    argument.UnpinManagedPointer();
            }
        
            if (method.hasReturnValue && !method.isAsync)
                StackPush(returnValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecutePushToStack(PushToStack pushToStack)
        {
            StackPush(pushToStack.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteExpressionCall(ExpressionCall expressionCall)
        {
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
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteSetSavePoint()
        {
            savePoint = offset;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
        {
            if (StackPop() == StackPop()) 
                return false;
        
            offset = jumpNotEquals.jumpOffset;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ExecuteJumpIfEquals(JumpEquals jumpEquals)
        {
            if (StackPop() != StackPop())
                return false;
        
            offset = jumpEquals.jumpOffset;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ExecuteJump(Jump jump)
        {
            offset = jump.jumpOffset;
            return true;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecutePushStringToStack(PushStringToStack pushStringToStack)
        {
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
            var registerIndex = storeToRegister.register;
            registers[registerIndex] = StackPop().longValue;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteLoadFromRegister(LoadFromRegister loadFromRegister)
        {
            var registerIndex = loadFromRegister.register;
            StackPush(registers[registerIndex]);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ExecuteDuplicateStack(DuplicateStack _)
        {
            StackPush(StackPeek());
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void StackPush(ScriptValue value) => stack.Push(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPop() => stack.Pop();
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue StackPeek() => stack.Peek();
	}
}