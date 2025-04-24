using System;
using System.IO;
using System.Threading.Tasks;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Threads;
using SV = DamnScript.Runtimes.VirtualMachines.ScriptValues.ScriptValue;
using SVP = DamnScript.Runtimes.VirtualMachines.ScriptValues.ScriptValuePtr;
using SVT = DamnScript.Runtimes.VirtualMachines.ScriptValues.ScriptValue.ValueType;

namespace DamnScript.Runtimes.BuiltIns
{
	public static class BuiltInMethods
	{
		public static void Register()
		{
#if !DAMN_SCRIPT_DISABLE_BUILTIN_METHODS
			ScriptEngine.RegisterNativeMethod((Action<SVP>)Log);
			ScriptEngine.RegisterNativeMethod((Action<SVP>)LogWarning);
			ScriptEngine.RegisterNativeMethod((Action<SVP>)LogError);
			
			ScriptEngine.RegisterNativeMethod((Action<SVP>)SetNoSavePointsEnable);
			ScriptEngine.RegisterNativeMethod((Action<SVP>)SetNoAwaitEnable);
			
			ScriptEngine.RegisterNativeMethod((Func<SVP>)GetCurrentThreadHandle);
			ScriptEngine.RegisterNativeMethod((Func<SVP>)GetCurrentThreadPtr);
			
			ScriptEngine.RegisterNativeMethod((Action<SVP, SVP>)RunLoadedScript);
			
			ScriptEngine.RegisterNativeMethod((Action<SVP>)StopThread);
			
			ScriptEngine.RegisterNativeMethod((Func<SVP, Task>)Delay);
			
			ScriptEngine.RegisterNativeMethod((Action)SetSavePoint);
			
			ScriptEngine.RegisterNativeMethod((Func<SVP>)SerializeToStreamAlloc);
			ScriptEngine.RegisterNativeMethod((Action<SVP>)SerializeToFile);
			
			ScriptEngine.RegisterNativeMethod((Action<SVP>)PushToStack);
			ScriptEngine.RegisterNativeMethod((Func<SVP>)PopFromStack);
			
			ScriptEngine.RegisterNativeMethod((Func<SVP, SVP>)ToString);
			ScriptEngine.RegisterNativeMethod((Func<SVP, SVP>)StringToNumber);
			ScriptEngine.RegisterNativeMethod((Func<SVP, SVP>)GetType);
#endif
		}

		private static void Log(SVP value) => Debugging.Log($"DS: {value.ToString()}");
		
		private static void LogWarning(SVP value) => Debugging.LogWarning($"DS: {value.ToString()}");
		
		private static void LogError(SVP value) => Debugging.LogError($"DS: {value.ToString()}");

		private static unsafe void SetNoSavePointsEnable(SVP value)
		{
			if (value.BoolValue)
				ScriptEngine.CurrentThreadPtr.value->threadParameters |= ThreadParameters.NoSavePoint;
			else
				ScriptEngine.CurrentThreadPtr.value->threadParameters &= ~ThreadParameters.NoSavePoint;
		}

		private static unsafe void SetNoAwaitEnable(SVP value)
		{
			if (value.BoolValue)
				ScriptEngine.CurrentThreadPtr.value->threadParameters |= ThreadParameters.NoAwait;
			else
				ScriptEngine.CurrentThreadPtr.value->threadParameters &= ~ThreadParameters.NoAwait;
		}

		private static SVP GetCurrentThreadHandle() =>
			new SV(ScriptEngine.CurrentThreadHandle.threadId).Return();
		
		private static unsafe SVP GetCurrentThreadPtr() =>
			new SV(ScriptEngine.CurrentThreadPtr.value, SVT.Pointer).Return();

		private static unsafe void RunLoadedScript(SVP scriptName, SVP regionName)
		{
			var nameString = scriptName.GetStringWrapper();
			var scriptDataPtr = ScriptEngine.GetScriptDataFromCache(nameString.ToString32()).value;
			if (scriptDataPtr == null)
				throw new Exception($"Script {nameString} not found in cache.");
			
			var regionString = regionName.GetStringWrapper();
			ScriptEngine.RunThread(scriptDataPtr, regionString.ToString32());
			
			nameString.Dispose();
			regionString.Dispose();
		}

		private static unsafe void StopThread(SVP threadHandleId)
		{
			var id = threadHandleId.LongValue;
			var vm = ScriptEngine.mainPtr.value;
			if (id < 0 || id >= vm->threads.Length)
				throw new Exception($"Thread handle {id} is out of range.");
			
			var thread = vm->threads.Begin + id;
			if (!thread->isAlive)
				throw new Exception($"Thread handle {id} is not alive.");
			
			thread->Dispose();
		}
		
		private static async Task Delay(SVP value) => 
			await Task.Delay(value.IntValue);

		private static unsafe void SetSavePoint() =>
			ScriptEngine.CurrentThreadPtr.value->ExecuteSetSavePoint();
		
		private static SVP SerializeToStreamAlloc()
		{
			var stream = ScriptEngine.SerializeToSerializationStream();
#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
			return SV.FromStructAlloc(stream).Return();
#else
			return SV.FromReferencePin(stream).Return();
#endif
		}

		// Rietmon: TODO Make split work for SafeString and System.String
		private static void SerializeToFile(SVP filePath)
		{
			var stream = ScriptEngine.SerializeToSerializationStream();
			var pathString = filePath.GetStringWrapper();
			var path = filePath.ToString();
			if (string.IsNullOrEmpty(path))
				throw new Exception("File path is empty.");
			
			var fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
			fileStream.Write(stream.AsSpan());
			fileStream.Flush();
			fileStream.Dispose();
			
			pathString.Dispose();
			stream.Dispose();
		}
		
		private static unsafe void PushToStack(SVP value)
		{
			var thread = ScriptEngine.CurrentThreadPtr.value;
			thread->returnValue = *value.value;
		}
		
		private static unsafe SVP PopFromStack()
		{
			var thread = ScriptEngine.CurrentThreadPtr.value;
			var value = thread->StackPop();
			return value.Return();
		}
		
		private static SVP ToString(SVP value)
		{
			var str = value.ToString();
			return SV.FromReferencePin(str).Return();
		}

		private static unsafe SVP StringToNumber(SVP value)
		{
			var ptr = value.value;
			switch (ptr->type)
			{
				case SVT.Integer:
					return new SV(ptr->longValue).Return();
				case SVT.Float32:
					return new SV(ptr->floatValue).Return();
				case SVT.Float64:
					return new SV(ptr->doubleValue).Return();
				case SVT.NativeStringPointer or SVT.ReferenceUnsafePointer or SVT.ReferenceSafePointer:
				{
					var str = ptr->ToString();
					if (double.TryParse(str, out var number))
						return new SV(number).Return();
					break;
				}
			}

			throw new Exception($"Unable to convert {value.ToString()} to number.");
		}
		
		private static unsafe SVP GetType(SVP value)
		{
			var ptr = value.value;
			return new SV((long)ptr->type).Return();
		}
	}
}