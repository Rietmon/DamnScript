using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
using PinHandle = DamnScript.Runtimes.Cores.Pins.DSObjectPin;
#endif

namespace DamnScript.Runtimes.Natives
{
    /// <summary>
    /// This struct is a wrapper to handle any type of value in the DamnScript.
    /// It has fixed size and can be used in the virtual machine.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public unsafe struct ScriptValue
    {
        private const string ExceptionMessageInvalidTypeForPointers = 
            $"Invalid type! Expected {nameof(ValueType.Pointer)}, " +
            $"{nameof(ValueType.ReferenceSafePointer)} or " +
            $"{nameof(ValueType.ReferenceUnsafePointer)}!";
        
        [FieldOffset(0)] public ValueType type;
        [FieldOffset(4)] public bool boolValue;
        [FieldOffset(4)] public byte byteValue;
        [FieldOffset(4)] public short shortValue;
        [FieldOffset(4)] public int intValue;
        [FieldOffset(4)] public long longValue;
        [FieldOffset(4)] public float floatValue;
        [FieldOffset(4)] public double doubleValue;
        [FieldOffset(4)] public char charValue;
        [FieldOffset(4)] public void* pointerValue;
        [FieldOffset(4)] public PinHandle safeValue;
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(ValueType type, long value)
        {
            this.type = type;
            longValue = value;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(bool value) => (type, boolValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(byte value) => (type, byteValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(short value) => (type, shortValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(int value) => (type, intValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(long value) => (type, longValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(float value) => (type, floatValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(double value) => (type, doubleValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(char value) => (type, charValue) = (ValueType.Primitive, value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(void* value, ValueType type)
        {
            this.type = type;
            pointerValue = value;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ScriptValue(PinHandle value) => (type, safeValue) = (ValueType.ReferenceSafePointer, value);
        
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
            UnsafeUtilities.Memcpy(UnsafeUtilities.AsPointer(ref value), allocate, sizeof(T));
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
                case ValueType.Pointer:
                {
                    var value = (UnsafeString*)pointerValue;
                    return value;
                }
                case ValueType.ReferenceUnsafePointer:
                {
                    var value = UnsafeUtilities.PointerToReference<string>(pointerValue);
                    return value;
                }
                case ValueType.ReferenceSafePointer:
                {
                    var value = UnsafeUtilities.Unpin<string>(safeValue);
                    return value;
                }
                default:
                    throw new Exception(ExceptionMessageInvalidTypeForPointers);
            }
        }

        /// <summary>
        /// Convert ANY value to SafeString and returns it.
        /// </summary>
        /// <returns>SafeString value</returns>
        /// <exception cref="Exception">If ScriptValue initialized incorrectly it will throw an exception</exception>
        public SafeString ToSafeString() =>
            type switch
            {
                ValueType.Primitive => longValue.ToString(),
                ValueType.Pointer or ValueType.ReferenceUnsafePointer or ValueType.ReferenceSafePointer => GetSafeString(),
                _ => throw new Exception("Type is invalid!")
            };

        /// <summary>
        /// Convert ANY value to SafeString and then to string and returns it.
        /// </summary>
        /// <returns>String value</returns>
        /// <exception cref="Exception">If ScriptValue initialized incorrectly it will throw an exception</exception>
        public override string ToString() =>
            type switch
            {
                ValueType.Primitive => longValue.ToString(),
                ValueType.Pointer or ValueType.ReferenceUnsafePointer or ValueType.ReferenceSafePointer => GetSafeString().ToString(),
                _ => throw new Exception("Type is invalid!")
            };

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
        public void* GetInstanceForVirtualMachine() => type switch
        {
            ValueType.Pointer or ValueType.ReferenceUnsafePointer => pointerValue,
            ValueType.ReferenceSafePointer => UnsafeUtilities.AddressOfPinned(safeValue),
            _ => throw new Exception(ExceptionMessageInvalidTypeForPointers)
        };
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(ScriptValue left, ScriptValue right) => 
            left.type == right.type && left.longValue == right.longValue;
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(ScriptValue left, ScriptValue right) => 
            left.type != right.type || left.longValue != right.longValue;
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(ScriptValue left, ScriptValue right) => left.longValue > right.longValue;
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(ScriptValue left, ScriptValue right) => left.longValue < right.longValue;
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(ScriptValue left, ScriptValue right) => left.longValue >= right.longValue;
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(ScriptValue left, ScriptValue right) => left.longValue <= right.longValue;
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator +(ScriptValue left, ScriptValue right) => new(left.type, left.longValue + right.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator -(ScriptValue left, ScriptValue right) => new(left.type, left.longValue - right.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator *(ScriptValue left, ScriptValue right) => new(left.type, left.longValue * right.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator /(ScriptValue left, ScriptValue right) => new(left.type, left.longValue / right.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator %(ScriptValue left, ScriptValue right) => new(left.type, left.longValue % right.longValue);
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator &(ScriptValue left, ScriptValue right) => new(left.type, left.longValue & right.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator |(ScriptValue left, ScriptValue right) => new(left.type, left.longValue | right.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator ^(ScriptValue left, ScriptValue right) => new(left.type, left.longValue ^ right.longValue);
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator <<(ScriptValue left, int right) => new(left.type, left.longValue << right);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator >>(ScriptValue left, int right) => new(left.type, left.longValue >> right);
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator ++(ScriptValue value) => new(value.type, value.longValue + 1);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator --(ScriptValue value) => new(value.type, value.longValue - 1);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator -(ScriptValue value) => new(value.type, -value.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue operator +(ScriptValue value) => new(value.type, +value.longValue);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(bool value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(byte value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(short value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(int value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(long value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(float value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(double value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(char value) => new(value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(void* value) => new(value, ValueType.Pointer);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ScriptValue(PinHandle value) => new(value);
    
        public enum ValueType
        {
            /// <summary>
            /// Represent that ScriptValue initialized incorrectly.
            /// </summary>
            Invalid,
        
            /// <summary>
            /// Represent that ScriptValue is a primitive type (byte, int, long, etc.)
            /// </summary>
            Primitive,
            /// <summary>
            /// Pointer to the ANY value.
            /// </summary>
            Pointer,
        
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