using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.OpCodes;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

// ReSharper disable EqualExpressionComparison

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	public unsafe partial struct VirtualMachineThread
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecuteNativeCall(NativeCall nativeCall)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Begin native call...");
#endif
			var methodName = metadata->GetMethodName(nativeCall.MethodIndex)->ToString32();
			var argumentsCount = nativeCall.ArgumentsCount;
			if (!MethodsStorage.TryGetNativeMethod(methodName, argumentsCount, out var method))
				throw new Exception($"Method \"{methodName}\" with {argumentsCount} arguments not found!");

#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Found native method \"{methodName}\" with {argumentsCount} arguments.");
#endif

			var argumentsStack = parametersStack.BeginPtr;
			UnsafeUtilities.Memset(argumentsStack, 0,
				ScriptValue.Size * VirtualMachineThreadParametersStack.MaxParameters);
			for (var i = method.argumentsCount - 1; i >= 0; i--)
				argumentsStack[i] = StackPop();

			// Rietmon: Result will be in the "returnValue" field
			ScriptValue.ReturnValuePtr = UnsafeUtilities.AsPointer(ref returnValue);
			VirtualMachineInvokeHelper.Invoke(method, argumentsStack, out var task);

			if (task != null)
			{
				if ((threadParameters & ThreadParameters.NoAwait) != 0)
				{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
					Debugging.Log($"Skipping awaiting async");
#endif
				}
				else
				{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
					Debugging.Log($"Method is async, pin result...");
#endif
					awaitTaskPin = UnsafeUtilities.Pin(task);
				}
			}

#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Free arguments stack...");
#endif

			if (method is { HasReturnValue: true, IsAsync: false })
			{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
				Debugging.Log($"Push return value ({returnValue.type}):({returnValue.longValue}) to stack...");
#endif
				StackPush(returnValue);
			}

			ScriptValue.ReturnValuePtr = null;

#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"End native call.");
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecutePushToStack(PushToStack pushToStack)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Push to stack ({pushToStack.value})");
#endif
			StackPush(pushToStack.value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecuteExpressionCall(ExpressionCall expressionCall)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Begin expression call ({expressionCall.type})...");
#endif
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
				case ExpressionCall.ExpressionCallType.And:
					StackPush(StackPop() != 0 && StackPop() != 0 ? 1 : 0); break;
				case ExpressionCall.ExpressionCallType.Or: StackPush(StackPop() != 0 || StackPop() != 0 ? 1 : 0); break;
				case ExpressionCall.ExpressionCallType.Not: StackPush(StackPop() == 0 ? 1 : 0); break;
				case ExpressionCall.ExpressionCallType.Test: StackPush(StackPop() != 0 ? 1 : 0); break;
				default: throw new ArgumentOutOfRangeException($"{nameof(expressionCall)} == {expressionCall.type}");
			}
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"End expression call");
#endif
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecuteSetSavePoint()
		{
			if ((threadParameters & ThreadParameters.NoSavePoint) != 0)
			{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
				Debugging.Log($"Skipping save point");
#endif
				return;
			}

#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Set save point...");
#endif

#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
			if (stack.stackOffset != 0)
				throw new Exception($"Attempt to set save point with non-empty stack!");
#endif
			savePoint = offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ExecuteJumpNotEquals(JumpNotEquals jumpNotEquals)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Begin Jump not equals...");
#endif
			if (StackPop() == StackPop())
				return false;

#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Jump to {offset}");
#endif
			offset = jumpNotEquals.jumpOffset;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ExecuteJumpIfEquals(JumpEquals jumpEquals)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Begin Jump equals...");
#endif
			if (StackPop() != StackPop())
				return false;

#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Jump to {offset}");
#endif
			offset = jumpEquals.jumpOffset;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool ExecuteJump(Jump jump)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Jump to {offset}");
#endif
			offset = jump.jumpOffset;
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecutePushStringToStack(PushStringToStack pushStringToStack)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Push string to stack...");
#endif
			var index = pushStringToStack.index;
			var str = metadata->GetNativeString(index);
			if (str == null)
				throw new Exception($"String not found by index: {index}");

			StackPush(str);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecuteStoreToRegister(StoreToRegister storeToRegister)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log("Begin store to register...");
#endif
			var registerIndex = storeToRegister.register;
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Store to ({registerIndex}) register...");
#endif
			registers[registerIndex] = StackPop().longValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecuteLoadFromRegister(LoadFromRegister loadFromRegister)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log("Begin load from register...");
#endif
			var registerIndex = loadFromRegister.register;
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"Load from ({registerIndex}) register...");
#endif
			StackPush(registers[registerIndex]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ExecuteDuplicateStack(DuplicateStack _)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log("Begin duplicate stack...");
#endif
			StackPush(StackPeek());
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void StackPush(ScriptValue value)
		{
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"STACK: Push value ({value.type}):({value.longValue}) to stack...");
#endif
			stack.Push(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ScriptValue StackPop()
		{
			var value = stack.Pop();
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"STACK: Pop value ({value.type}):({value.longValue}) from stack...");
#endif
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ScriptValue StackPeek()
		{
			var value = stack.Peek();
#if DAMN_SCRIPT_ENABLE_EXECUTION_LOG
			Debugging.Log($"STACK: Peek value ({value.type}):({value.longValue}) from stack...");
#endif
			return value;
		}
	}
}