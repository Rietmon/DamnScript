using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Natives
{
	public unsafe partial struct ScriptValue
	{
		public T GetReference<T>() where T : class => type switch
		{
			ValueType.ReferenceSafePointer => GetReferencePin<T>(),
#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
			ValueType.ReferenceUnsafePointer => GetReferenceUnsafe<T>(),
#endif
#if !DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
			_ => throw new NotSupportedException("For GetReference use only " +
			                                     $"{nameof(ValueType.ReferenceSafePointer)}!")
#else
			_ => throw new NotSupportedException("For GetReference use only " +
			                                     $"{nameof(ValueType.ReferenceSafePointer)} or " +
			                                     $"{nameof(ValueType.ReferenceUnsafePointer)}!")
#endif
		};
		
		/// <summary>
		/// Get reference from the safe pointer and can unpin it.
		/// </summary>
		/// <typeparam name="T">Type of the reference</typeparam>
		/// <returns>Reference from the pointer</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetReferencePin<T>() where T : class
		{
			if (type != ValueType.ReferenceSafePointer)
				throw new NotSupportedException("For GetStruct use only " +
				                                $"{nameof(ValueType.Pointer)}!");
            
			var value = (T)safeValue.Target;
			return value;
		}

#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
		/// <summary>
		/// Get reference from the unsafe pointer.
		/// </summary>
		/// <typeparam name="T">Type of the reference</typeparam>
		/// <returns>Reference from the pointer</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetReferenceUnsafe<T>() where T : class
		{
			if (type != ValueType.ReferenceUnsafePointer)
				throw new NotSupportedException("For GetReferenceUnsafe use only " +
				                                $"{nameof(ValueType.ReferenceUnsafePointer)}!");
			
			var value = UnsafeUtilities.PointerToReference<T>(pointerValue);
			return value;
		}
#endif
    
		
#if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
		/// <summary>
		/// Get struct value from the pointer and can free it.
		/// </summary>
		/// <typeparam name="T">Type of the struct</typeparam>
		/// <returns>Struct from the pointer</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T GetStruct<T>() where T : unmanaged
		{
			if (type != ValueType.Pointer)
				throw new NotSupportedException("For GetStruct use only " +
				                                $"{nameof(ValueType.Pointer)}!");
            
			var value = *(T*)pointerValue;
			return value;
		}
#endif

		/// <summary>
		/// If ScriptValue represents a managed reference value, it will return a pointer to it.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception">Will throw exception if you try to get safe string if ScriptValue is based on primitive or initialized incorrectly</exception>
		public void* GetReferencePointer() => type switch
		{
			ValueType.ReferenceUnsafePointer => pointerValue,
			ValueType.ReferenceSafePointer => safeValue.Address,
			_ => throw new NotSupportedException("For GetReferencePointer use only " +
			                                     $"{nameof(ValueType.ReferenceUnsafePointer)} or " +
			                                     $"{nameof(ValueType.ReferenceSafePointer)}!")
		};
		
        /// <summary>
        /// Safe way to get a StringWrapper from the ScriptValue.
        /// It works only if ScriptValue is based on pointers.
        /// If it is based on primitive, it will throw an exception.
        /// In the case of a Primitive type, you can use ToString() method.
        /// </summary>
        /// <returns>SafeString value</returns>
        /// <exception cref="Exception">Will throw exception if you try to get safe string if ScriptValue is based on primitive or initialized incorrectly</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public StringWrapper GetStringWrapper()
        {
            switch (type)
            {
                case ValueType.NativeStringPointer:
                {
                    var value = (NativeString*)pointerValue;
                    return value;
                }
                case ValueType.ReferenceUnsafePointer:
                {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
                    var obj = UnsafeUtilities.PointerToReference<object>(pointerValue);
                    if (obj is string str)
                        return str;
                    
                    throw new NotSupportedException($"Attempt to get SafeString from non-string value! If you want to convert into string, use ToString() method.");
#else
                    var value = UnsafeUtilities.PointerToReference<string>(pointerValue);
                    return value;
#endif
                }
                case ValueType.ReferenceSafePointer:
                {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
	                if (safeValue.Target is string)
		                return safeValue;
                    
                    throw new NotSupportedException($"Attempt to get SafeString from non-string value! If you want to convert into string, use ToString() method.");
#else
                    return safeValue;
#endif
                }
                default:
                    throw new NotSupportedException("For GetSafeString use only " +
                                                    $"{nameof(ValueType.NativeStringPointer)}, " +
                                                    $"{nameof(ValueType.ReferenceUnsafePointer)} or " +
                                                    $"{nameof(ValueType.ReferenceSafePointer)}!");
            }
        }
        
        /// <summary>
        /// Convert ANY value to SafeString and then to string and returns it.
        /// </summary>
        /// <returns>String value</returns>
        /// <exception cref="Exception">If ScriptValue initialized incorrectly or attempt to convert an anonymous pointer, it will throw an exception</exception>
        public override string ToString()
        {
            switch (type)
            {
                case ValueType.Integer:
                {
                    return longValue.ToString();
                }
                case ValueType.Float32:
                {
                    return floatValue.ToString(CultureInfo.InvariantCulture);
                }
                case ValueType.Float64:
                {
                    return doubleValue.ToString(CultureInfo.InvariantCulture);
                }
                case ValueType.NativeStringPointer:
                {
                    return ((NativeString*)pointerValue)->ToString();
                }
                case ValueType.ReferenceUnsafePointer:
                {
                    var value = UnsafeUtilities.PointerToReference<object>(pointerValue);
                    if (value is string str)
                        return str;
                    
                    return value.ToString();
                }
                case ValueType.ReferenceSafePointer:
                {
                    var target = safeValue.Target;
                    if (target is string str)
                        return str;
                    
                    return target.ToString();
                }
                default:
                    throw new NotSupportedException("For ToString use only " +
                                                    $"{nameof(ValueType.Integer)}, " +
                                                    $"{nameof(ValueType.Float32)}, " +
                                                    $"{nameof(ValueType.Float64)}, " +
                                                    $"{nameof(ValueType.NativeStringPointer)}, " +
                                                    $"{nameof(ValueType.ReferenceUnsafePointer)} or " +
                                                    $"{nameof(ValueType.ReferenceSafePointer)}!");
            }
        }
	}
}