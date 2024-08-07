using System;
using System.Threading.Tasks;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScript.Runtimes.VirtualMachines
{
	public unsafe partial struct VirtualMachine
	{
		public void AddToAwait(Task result, VirtualMachineThreadPtr pointer) => 
			threadsAreAwait.Add(((IntPtr)UnsafeUtilities.ReferenceToPointer(result), pointer));
    
		public bool IsInAwait(VirtualMachineThreadPtr virtualMachineThreadPointer)
		{
			var begin = threadsAreAwait.Begin;
			var end = threadsAreAwait.End;
			while (begin < end)
			{
				if (begin->pointer.value == virtualMachineThreadPointer.value)
					return true;
            
				begin++;
			}

			return false;
		}

		public void RemoveFromAwait(VirtualMachineThreadPtr virtualMachineThreadPointer)
		{
			var begin = threadsAreAwait.Begin;
			var end = threadsAreAwait.End;
			var i = 0;
			while (begin < end)
			{
				if (begin->pointer.value == virtualMachineThreadPointer.value)
				{
					threadsAreAwait.RemoveAt(i);
					return;
				}
            
				begin++;
				i++;
			}
		}
	}
}