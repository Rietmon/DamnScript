using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Natives
{
    /// <summary>
    /// Using as an argument for the native methods be passed by reference.
    /// It is a pointer to the ScriptValue.
    /// </summary>
#if DAMN_SCRIPT_DISABLE_ALIGNMENT_SCRIPT_VALUE
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
#else
    [StructLayout(LayoutKind.Sequential)]
#endif
    public readonly unsafe struct ScriptValuePtr
    {
        public ScriptValue.ValueType Type => value->type;
        public bool BoolValue => value->boolValue;
        public byte ByteValue => value->byteValue;
        public sbyte SByteValue => value->sbyteValue;
        public short ShortValue => value->shortValue;
        public ushort UShortValue => value->ushortValue;
        public int IntValue => value->intValue;
        public uint UIntValue => value->uintValue;
        public long LongValue => value->longValue;
        public ulong ULongValue => value->ulongValue;
        public float FloatValue => (float)value->doubleValue;
        public double DoubleValue => value->doubleValue;
        public char CharValue => value->charValue;
        public void* PointerValue => value->pointerValue;
        public ObjectPin SafeValue => value->safeValue;
        
        public ref ScriptValue RefValue => ref UnsafeUtilities.AsRef<ScriptValue>(value);
        
        public readonly ScriptValue* value;
        
        public ScriptValuePtr(ScriptValue* value) => this.value = value;
        
        public ScriptValuePtr(ref ScriptValue value) => this.value = UnsafeUtilities.AsPointer(ref value);
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.GetReferencePin{T}"/>
        /// </summary>
        public T GetReferencePin<T>(bool freeBeforeReturn = true) where T : class => 
            value->GetReferencePin<T>(freeBeforeReturn);
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.GetReferenceUnsafe{T}"/>
        /// </summary>
        public T GetReferenceUnsafe<T>() where T : class => value->GetReferenceUnsafe<T>();
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.GetReference{T}"/>
        /// </summary>
        public T GetReference<T>() where T : class => value->GetReference<T>();
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.GetStruct{T}"/>
        /// </summary>
        public T GetStruct<T>(bool freeBeforeReturn = true) where T : unmanaged => value->GetStruct<T>(freeBeforeReturn);
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.GetSafeString"/>
        /// </summary>
        public SafeString GetSafeString() => value->GetSafeString();
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.GetReferencePointer"/>
        /// </summary>
        public void* GetReferencePointer() => value->GetReferencePointer();
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.UnpinManagedPointer"/>
        /// </summary>
        public void UnpinManagedPointer() => value->UnpinManagedPointer();
        
        /// <summary>
        /// <inheritdoc cref="ScriptValue.ToString"/>
        /// </summary>
        public override string ToString() => value->ToString();
        
        public static implicit operator ScriptValuePtr(ScriptValue* value) => new(value);
        public static implicit operator ScriptValue*(ScriptValuePtr ptr) => ptr.value;
    }
    
    /// <summary>
    /// This struct is a wrapper to handle any type of value in the DamnScript.
    /// It has a fixed size and can be used in the virtual machine.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public unsafe partial struct ScriptValue : IEquatable<ScriptValue>
    {
        public const int TypeSize = UnsafeUtilities.PointerSize;
        public const int Size = TypeSize + 8;
        
        public static ScriptValue* returnValuePtr;

        public float SafeFloatValue => type switch
        {
            ValueType.NumberInteger => longValue,
            ValueType.NumberFloat32 => floatValue,
            ValueType.NumberFloat64 => (float)doubleValue,
            _ => throw new NotSupportedException("ScriptValue is not a number type!")
        };
        
        public double SafeDoubleValue => type switch
        {
            ValueType.NumberInteger => longValue,
            ValueType.NumberFloat32 => floatValue,
            ValueType.NumberFloat64 => doubleValue,
            _ => throw new NotSupportedException("ScriptValue is not a number type!")
        };
        
        public long SafeIntegerValue => type switch
        {
            ValueType.NumberInteger => longValue,
            ValueType.NumberFloat32 => (long)floatValue,
            ValueType.NumberFloat64 => (long)doubleValue,
            _ => throw new NotSupportedException("ScriptValue is not a number type!")
        };
        
        [FieldOffset(0)] public ValueType type;
        [FieldOffset(TypeSize)] public bool boolValue;
        [FieldOffset(TypeSize)] public byte byteValue;
        [FieldOffset(TypeSize)] public sbyte sbyteValue;
        [FieldOffset(TypeSize)] public short shortValue;
        [FieldOffset(TypeSize)] public ushort ushortValue;
        [FieldOffset(TypeSize)] public int intValue;
        [FieldOffset(TypeSize)] public uint uintValue;
        [FieldOffset(TypeSize)] public long longValue;
        [FieldOffset(TypeSize)] public ulong ulongValue;
        [FieldOffset(TypeSize)] public float floatValue;
        [FieldOffset(TypeSize)] public double doubleValue;
        [FieldOffset(TypeSize)] public char charValue;
        [FieldOffset(TypeSize)] public void* pointerValue;
        [FieldOffset(TypeSize)] public ObjectPin safeValue;
        [FieldOffset(TypeSize)] public NativeString* nativeStringValue;
        
        /// <summary>
        /// Convert value to an internal constant pointer and use it as a return value.
        /// </summary>
        /// <returns>Pointer to returned value (use only in VM thread call)</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValuePtr Return()
        {
            *returnValuePtr = this;
            return new ScriptValuePtr(returnValuePtr);
        }
        
        /// <summary>
        /// Create a new ScriptValue from a reference type and pin it. Safest way to handle references.
        /// </summary>
        /// <param name="value">Managed reference</param>
        /// <typeparam name="T">Type of managed reference</typeparam>
        /// <returns>ScriptValue with a pointer to value</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue FromReferencePin<T>(T value) where T : class => 
            new(UnsafeUtilities.Pin(value));
        
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

        /// <summary>
        /// Get reference from the safe pointer and can unpin it.
        /// </summary>
        /// <param name="freeBeforeReturn">Does need to unpin reference?</param>
        /// <typeparam name="T">Type of the reference</typeparam>
        /// <returns>Reference from the pointer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetReferencePin<T>(bool freeBeforeReturn = true) where T : class
        {
            var value = (T)safeValue.Target;
            if (freeBeforeReturn)
                safeValue.Free();
            return value;
        }
    
        /// <summary>
        /// Get reference from the unsafe pointer.
        /// </summary>
        /// <typeparam name="T">Type of the reference</typeparam>
        /// <returns>Reference from the pointer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetReferenceUnsafe<T>() where T : class => UnsafeUtilities.PointerToReference<T>(pointerValue);
        
        /// <summary>
        /// Get reference
        /// </summary>
        /// <typeparam name="T">Type of the reference</typeparam>
        /// <returns>Reference from the pointer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetReference<T>() where T : class => type switch
        {
            ValueType.ReferenceUnsafePointer => GetReferenceUnsafe<T>(),
            ValueType.ReferenceSafePointer => GetReferencePin<T>(),
            _ => throw new NotSupportedException("For GetReference use only " +
                                                 $"{nameof(ValueType.ReferenceUnsafePointer)} or " +
                                                 $"{nameof(ValueType.ReferenceSafePointer)}!")
        };
    
        /// <summary>
        /// Get struct value from the pointer and can free it.
        /// </summary>
        /// <param name="freeBeforeReturn">Does need to free pointer after get a value?</param>
        /// <typeparam name="T">Type of the struct</typeparam>
        /// <returns>Struct from the pointer</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetStruct<T>(bool freeBeforeReturn = true) where T : unmanaged
        {
            var value = *(T*)pointerValue;
            if (freeBeforeReturn)
                UnsafeUtilities.Free(pointerValue);
            return value;
        }
    
        /// <summary>
        /// Safe way to get an SafeString from the ScriptValue.
        /// It works only if ScriptValue is based on pointers.
        /// If it is based on primitive, it will throw an exception.
        /// In case of a Primitive type, you can use ToSafeString() or ToString() method.
        /// </summary>
        /// <returns>SafeString value</returns>
        /// <exception cref="Exception">Will throw exception if you try to get safe string if ScriptValue is based on primitive or initialized incorrectly</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SafeString GetSafeString()
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
                    
                    throw new Exception($"Attempt to get SafeString from non-string value! If you want to convert into string, use ToString() method.");
#else
                    var value = UnsafeUtilities.PointerToReference<string>(pointerValue);
                    return value;
#endif
                }
                case ValueType.ReferenceSafePointer:
                {
                    var value = safeValue;
                    return value;
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
                case ValueType.NumberInteger:
                {
                    return longValue.ToString();
                }
                case ValueType.NumberFloat32:
                {
                    return floatValue.ToString(CultureInfo.InvariantCulture);
                }
                case ValueType.NumberFloat64:
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
                                                    $"{nameof(ValueType.NativeStringPointer)}, " +
                                                    $"{nameof(ValueType.ReferenceUnsafePointer)} or " +
                                                    $"{nameof(ValueType.ReferenceSafePointer)}!");
            }
        }
        
        public bool Equals(ScriptValue other) => Equal(this, other);

        public override bool Equals(object obj) => obj is ScriptValue other && Equals(other);

        public override int GetHashCode() => HashCode.Combine((int)type, longValue);
        
        /// <summary>
        /// Unpin safe pointer if a value type is it.
        /// </summary>
        public void UnpinManagedPointer()
        {
            if (type == ValueType.ReferenceSafePointer)
                safeValue.Free();
        }

        /// <summary>
        /// If ScriptValue represents a managed reference value, it will return a pointer to it.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Will throw exception if you try to get safe string if ScriptValue is based on primitive or initialized incorrectly</exception>
        public void* GetReferencePointer() => type switch
        {
            ValueType.ReferenceUnsafePointer => pointerValue,
            ValueType.ReferenceSafePointer => safeValue.Address,
            _ => throw new Exception("For GetReferencePointer use only " +
                                     $"{nameof(ValueType.ReferenceUnsafePointer)} or " +
                                     $"{nameof(ValueType.ReferenceSafePointer)}!")
        };

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
            /// Represent that ScriptValue initialized as an integer.
            /// </summary>
            NumberInteger,
            
            /// <summary>
            /// Represent that ScriptValue initialized as a float.
            /// </summary>
            NumberFloat32,
            /// <summary>
            /// Represent that ScriptValue initialized as a double.
            /// </summary>
            NumberFloat64,
            
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