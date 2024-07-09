using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;

#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
using PinHandle = DamnScript.Runtimes.Cores.DSObjectPin;
#endif

namespace DamnScript.Runtimes.Natives
{
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public unsafe struct ScriptValue
    {
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

    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue FromReferencePin<T>(T value) where T : class => 
            new(UnsafeUtilities.Pin(value));
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue FromReferenceUnsafe<T>(T value) where T : class => 
            new(UnsafeUtilities.ReferenceToPointer(value), ValueType.ReferenceUnsafePointer);
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ScriptValue FromStructAlloc<T>(T value) where T : unmanaged
        {
            var allocate = UnsafeUtilities.Alloc(sizeof(T));
            UnsafeUtilities.Memcpy(UnsafeUtilities.AsPointer(ref value), allocate, sizeof(T));
            return new ScriptValue(allocate, ValueType.Pointer);
        }

    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetReferencePin<T>(bool freeBeforeReturn = true) where T : class
        {
            var value = (T)safeValue.Target;
            if (freeBeforeReturn)
                safeValue.Free();
            return value;
        }
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetReferenceUnsafe<T>() where T : class => UnsafeUtilities.PointerToReference<T>(pointerValue);
    
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetStruct<T>(bool freeBeforeReturn = true) where T : unmanaged
        {
            var value = *(T*)pointerValue;
            if (freeBeforeReturn)
                UnsafeUtilities.Free(pointerValue);
            return value;
        }
    
    
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
                    var value = (string)safeValue.Target;
                    safeValue.Free();
                    return value;
                }
                default:
                    throw new Exception("Invalid type for SafeString! Expected Pointer or Safe!");
            }
        }

        public SafeString ToSafeString()
        {
            return type switch
            {
                ValueType.Primitive => longValue.ToString(),
                ValueType.Pointer or ValueType.ReferenceUnsafePointer or ValueType.ReferenceSafePointer => GetSafeString(),
                _ => throw new Exception("Invalid type of ScriptValue!")
            };
        }
    
        public void UnpinManagedPointer() => safeValue.Free();
    
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
            Invalid,
        
            Primitive,
            Pointer,
        
            ReferenceUnsafePointer,
            ReferenceSafePointer
        }
    }
}