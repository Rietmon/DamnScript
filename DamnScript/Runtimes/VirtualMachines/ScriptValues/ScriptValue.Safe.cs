using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.VirtualMachines.ScriptValues
{
	public partial struct ScriptValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue FromReferencePin(object value, bool isPersistent = false)
		{
			var pin = UnsafeUtilities.Pin(value);
			return new ScriptValue(pin, isPersistent);
		}
	}
}