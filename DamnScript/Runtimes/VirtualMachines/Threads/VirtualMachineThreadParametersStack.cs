using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct VirtualMachineThreadParametersStack
	{
		public const int MaxParameters = 10;

		public ParametersBuffer parameters;

		public ScriptValue* BeginPtr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (ScriptValue*)UnsafeUtilities.AsPointer(ref parameters);
		}
	
		[StructLayout(LayoutKind.Sequential, Size = MaxParameters * ScriptValue.Size)]
		public struct ParametersBuffer { }
	}
}