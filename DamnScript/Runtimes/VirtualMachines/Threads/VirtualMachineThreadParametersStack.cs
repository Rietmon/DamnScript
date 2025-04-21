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
		
		public ScriptValue p1;
		public ScriptValue p2;
		public ScriptValue p3;
		public ScriptValue p4;
		public ScriptValue p5;
		public ScriptValue p6;
		public ScriptValue p7;
		public ScriptValue p8;
		public ScriptValue p9;
		public ScriptValue p10;

		public ScriptValue* BeginPtr
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => UnsafeUtilities.AsPointer(ref p1);
		}
	}
}