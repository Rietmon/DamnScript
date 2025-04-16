using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.VirtualMachines.ScriptValues
{
	public unsafe partial struct ScriptValue
	{
#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
		/// <summary>
		/// Convert a reference to pointer and create a new ScriptValue from it.
		/// </summary>
		/// <param name="value">Managed reference</param>
		/// <typeparam name="T">Type of managed reference</typeparam>
		/// <returns>ScriptValue with a pointer to value</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue FromReferenceUnsafe<T>(T value) where T : class => 
			new(UnsafeUtilities.ReferenceToPointer(value), ValueType.ReferenceUnsafePointer);
		
		/// <summary>
		/// Alloc copy of the struct and create a new ScriptValue from it.
		/// </summary>
		/// <param name="value">Struct value</param>
		/// <typeparam name="T">Type of the struct</typeparam>
		/// <returns>ScriptValue with a pointer to value</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ScriptValue FromStructAlloc<T>(T value) where T : unmanaged
		{
			var allocate = UnsafeUtilities.Alloc(sizeof(T));
			UnsafeUtilities.Memcpy(&value, allocate, sizeof(T));
			return new ScriptValue(allocate, ValueType.Pointer);
		}
#endif
	}
}