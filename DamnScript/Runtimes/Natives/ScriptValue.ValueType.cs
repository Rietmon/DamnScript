namespace DamnScript.Runtimes.Natives
{
	public partial struct ScriptValue
	{
		public enum ValueType :
#if DAMN_SCRIPT_SCRIPT_VALUE_SIZE_12
            int
#else
			long
#endif
		{
			/// <summary>
			/// Represent that ScriptValue initialized incorrectly.
			/// </summary>
			Invalid,
            
			/// <summary>
			/// Represent that ScriptValue initialized as an ANY integer.
			/// </summary>
			Integer,
            
			/// <summary>
			/// Represent that ScriptValue initialized as a float.
			/// </summary>
			Float32,
			/// <summary>
			/// Represent that ScriptValue initialized as a double.
			/// </summary>
			Float64,
            
			/// <summary>
			/// Pointer to any unmanaged value.
			/// </summary>
			Pointer,
            
			/// <summary>
			/// Pointer to the native string.
			/// </summary>
			NativeStringPointer,

			/// <summary>
			/// Unsafe pointer to the reference type.
			/// </summary>
			ReferenceUnsafePointer,
			/// <summary>
			/// Safe pointer to the reference type.
			/// </summary>
			ReferenceSafePointer
		}
	}
}