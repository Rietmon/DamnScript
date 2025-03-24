using System;
using System.IO;
using System.Threading.Tasks;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.VirtualMachines.Threads;
using SV = DamnScript.Runtimes.Natives.ScriptValue;
using SVP = DamnScript.Runtimes.Natives.ScriptValuePtr;
using SVT = DamnScript.Runtimes.Natives.ScriptValue.ValueType;

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
			ScriptEngine.RegisterNativeMethod((Func<SVP>)GetCurrentThreadHandle);
			ScriptEngine.RegisterNativeMethod((Func<SVP>)GetCurrentThreadPtr);
			ScriptEngine.RegisterNativeMethod((Action<SVP, SVP>)RunLoadedScript);
			ScriptEngine.RegisterNativeMethod((Action<SVP>)StopThread);
			ScriptEngine.RegisterNativeMethod((Func<SVP, Task>)Delay);
			ScriptEngine.RegisterNativeMethod((Action)SetSavePoint);
			ScriptEngine.RegisterNativeMethod((Func<SVP>)SerializeToStreamAlloc);
			ScriptEngine.RegisterNativeMethod((Action<SVP>)SerializeToFile);
#endif
		}

		private static void Log(SVP value) => Debugging.Log($"DS: {value.ToString()}");
		
		private static void LogWarning(SVP value) => Debugging.LogWarning($"DS: {value.ToString()}");
		
		private static void LogError(SVP value) => Debugging.LogError($"DS: {value.ToString()}");

		private static SVP GetCurrentThreadHandle() =>
			new SV(ScriptEngine.CurrentThreadHandle.threadId).Return();
		
		private static unsafe SVP GetCurrentThreadPtr() =>
			new SV(ScriptEngine.CurrentThreadPtr.value, SVT.Pointer).Return();

		private static unsafe void RunLoadedScript(SVP scriptName, SVP regionName)
		{
			var nameString = scriptName.GetSafeString();
			var scriptDataPtr = ScriptEngine.GetScriptDataFromCache(nameString.ToString32()).value;
			if (scriptDataPtr == null)
				throw new Exception($"Script {nameString} not found in cache.");
			
			var regionString = regionName.GetSafeString();
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
			return SV.FromStructAlloc(stream).Return();
		}

		// Rietmon: TODO Make split work for SafeString and System.String
		private static void SerializeToFile(SVP filePath)
		{
			var stream = ScriptEngine.SerializeToSerializationStream();
			var pathString = filePath.GetSafeString();
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
	}
}