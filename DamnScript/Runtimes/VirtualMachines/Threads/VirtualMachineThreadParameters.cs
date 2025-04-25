using System;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	[Flags]
	public enum VirtualMachineThreadParameters
	{
		None = 0,
		NoAwait = 1 << 0,
		NoSavePoint = 1 << 1,
	}
}