#define DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
using System;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	public readonly unsafe struct VirtualMachineThreadHandle
	{
		public VirtualMachineThreadPtr Ptr => 
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
			virtualMachinePtr.value->IsAlive 
				? new VirtualMachineThreadPtr(virtualMachinePtr.value->threads.Begin + threadId) 
				: throw new InvalidOperationException("Virtual machine is not alive!");
#else
			new(virtualMachinePtr.value->threads.Begin + threadId);
#endif
		
		public readonly long threadId;
		public readonly VirtualMachinePtr virtualMachinePtr;

		public VirtualMachineThreadHandle(long threadId, VirtualMachinePtr virtualMachinePtr)
		{
			this.threadId = threadId;
			this.virtualMachinePtr = virtualMachinePtr;
		}
	}
}