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
				currentThread = begin;
				if (!IsInAwait(begin))
				{
					while (true)
					{
						if (!begin->ExecuteNext())
							break;
						
						if (begin->awaitTaskPin != default)
							break;
					}
					
					if (begin->awaitTaskPin == default)
						threads.RemoveAt((int)(end - 1 - begin));
				}
				else
				{
					var result = (Task)begin->awaitTaskPin.Target;
					if (result.IsCompleted)
					{
						begin->awaitTaskPin.Free();
						begin->awaitTaskPin = default;
						if (result is Task<ScriptValue> task)
							begin->StackPush(task.Result);
					}
				}
            
				begin++;
			}
		}
	}
}