using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	/// <summary>
	/// Virtual machine thread handle.
	/// Because of reallocation of the thread array, this handle is a safe way to access the real thread pointer.
	/// Please do not cache pointers to the thread, because it can be reallocated.
	/// </summary>
	public readonly unsafe struct VirtualMachineThreadHandle
	{
		/// <summary>
		/// Pointer to the thread.
		/// </summary>
		public VirtualMachineThreadPtr Ptr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => 
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
				virtualMachinePtr.value->IsAlive 
					? new VirtualMachineThreadPtr(virtualMachinePtr.value->threads.Begin + threadId) 
					: throw new InvalidOperationException("Virtual machine is not alive!");
#else
				new(virtualMachinePtr.value->threads.Begin + threadId);
#endif
		}
		
		public readonly long threadId;
		public readonly VirtualMachinePtr virtualMachinePtr;

		public VirtualMachineThreadHandle(long threadId, VirtualMachinePtr virtualMachinePtr)
		{
			this.threadId = threadId;
			this.virtualMachinePtr = virtualMachinePtr;
		}
	}
}