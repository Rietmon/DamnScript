using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Natives
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