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
			ExecuteAwaitsThreads();
			ExecuteThreads();
        
			return HasThreads || HasThreadsAwaiting;
		}
    
		private void ExecuteAwaitsThreads()
		{
			var begin = threadsAreAwait.Begin;
			var end = threadsAreAwait.End;
			while (begin < end)
			{
				currentThread = begin->pointer;
				var result = UnsafeUtilities.PointerToReference<IAsyncResult>(begin->result.ToPointer());
				if (result.IsCompleted)
				{
					RemoveFromAwait(begin->pointer);
					if (result is Task<ScriptValue> task)
						begin->pointer.value->StackPush(task.Result.longValue);
				}
            
				begin++;
			}
		}
    
		private void ExecuteThreads()
		{
			var begin = threads.Begin;
			var end = threads.End;
			while (begin < end)
			{
				currentThread = begin;
				if (!IsInAwait(begin))
				{
					Task result;
					while (begin->ExecuteNext(out result))
					{
						if (result == null)
							continue;

						AddToAwait(result, begin);
						break;
					}

					if (result == null)
						threads.RemoveAt((int)(end - 1 - begin));
				}
            
				begin++;
			}
		}
	}
}