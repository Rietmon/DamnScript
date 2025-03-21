using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.VirtualMachines.Threads
{
	public unsafe struct VirtualMachineThreadParametersStack
	{
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
		
		public ScriptValue* BeginPtr => UnsafeUtilities.AsPointer(ref p1);
	}
}