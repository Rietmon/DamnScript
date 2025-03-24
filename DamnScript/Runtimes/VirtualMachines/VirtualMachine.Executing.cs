using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.VirtualMachines
{
	public unsafe partial struct VirtualMachine
	{
		public bool ExecuteNext()
		{
			ExecuteThreads();

			HasThreads = false;
			var begin = threads.Begin;
			var end = threads.End;
			while (begin < end)
			{
				if (begin->isAlive)
				{
					HasThreads = true;
					break;
				}

				begin++;
			}
			return HasThreads;
		}
    
		private void ExecuteThreads()
		{
			var begin = threads.Begin;
			var end = threads.End;
			while (begin < end)
			{
				if (!begin->isAlive)
				{
					begin++;
					continue;
				}

				currentThread = begin; 
				
				executeThreadProcedure:
				if (begin->awaitTaskPin == default)
				{
					while (true)
					{
						if (!begin->ExecuteNextOpCode())
							break;
						
						if (begin->awaitTaskPin != default)
							break;
					}

					if (begin->awaitTaskPin == default)
						begin->Dispose();
				}
				else
				{
					var result = (Task)begin->awaitTaskPin.Target;
					if (result.IsCompleted)
					{
						begin->awaitTaskPin.Free();
						begin->awaitTaskPin = default;
						begin->UnpinParameters();
						if (result is Task<ScriptValuePtr> task)
						{
							var scriptValue = *task.Result.value;
#if !DAMN_SCRIPT_ENBALE_MONO && !DAMN_SCRIPT_DISABLE_ASYNC_PINNING
							if (scriptValue.type == ScriptValue.ValueType.ReferenceUnsafePointer)
							{
								throw new Exception($"Async ({result}) method in .NET should use Pinned return value be cause GC can move it! " +
								                    "If you are sure what you are doing, please enable DAMN_SCRIPT_DISABLE_ASYNC_PINNING.");
							}
#endif
							begin->StackPush(scriptValue);
						}
						goto executeThreadProcedure;
					}
				}
            
				begin++;
			}
		}
	}
}