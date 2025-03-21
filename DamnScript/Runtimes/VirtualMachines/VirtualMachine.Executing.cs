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
		
		public bool IsInAwait(VirtualMachineThreadPtr virtualMachineThreadPointer) => 
			virtualMachineThreadPointer.value->awaitTaskPin != default;
    
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
				if (!IsInAwait(begin))
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
						if (result is Task<ScriptValuePtr> task)
							begin->StackPush(*task.Result.value);
						goto executeThreadProcedure;
					}
				}
            
				begin++;
			}
		}
	}
}