using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.VirtualMachines.ScriptValues
{
	public partial struct ScriptValue
	{
		public static ScriptValue FromReferencePin(object value)
		{
			var pin = UnsafeUtilities.Pin(value);
			return new ScriptValue(pin);
		}
	}
}