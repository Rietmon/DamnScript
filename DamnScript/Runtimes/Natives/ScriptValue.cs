using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Natives;

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
    [FieldOffset(4)] public GCHandle safeValue;
    
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
    public ScriptValue(void* value)
    {
        type = ValueType.Pointer;
        pointerValue = value;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ScriptValue(GCHandle value) => (type, safeValue) = (ValueType.Safe, value);

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScriptValue FromReferencePin<T>(T value) where T : class => new(GCHandle.Alloc(value, GCHandleType.Pinned));
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScriptValue FromReference<T>(T value) where T : class => new(UnsafeUtilities.ReferenceToPointer(value));
    
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ScriptValue FromStructAlloc<T>(T value) where T : unmanaged
    {
        var allocate = UnsafeUtilities.Alloc(sizeof(T));
        UnsafeUtilities.Memcpy(allocate, Unsafe.AsPointer(ref value), sizeof(T));
        return new ScriptValue(allocate);
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
    public T GetReference<T>() where T : class => UnsafeUtilities.PointerToReference<T>(pointerValue);
    
    
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
            case ValueType.ReferencePointer:
            {
                var value = UnsafeUtilities.PointerToReference<string>(pointerValue);
                return value;
            }
            case ValueType.Safe:
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
            ValueType.Pointer or ValueType.ReferencePointer or ValueType.Safe => GetSafeString(),
            _ => throw new Exception("Invalid type of ScriptValue!")
        };
    }
    
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
    public static implicit operator ScriptValue(void* value) => new(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator ScriptValue(GCHandle value) => new(value);
    
    public enum ValueType
    {
        Invalid,
        Primitive,
        Pointer,
        ReferencePointer,
        Safe
    }
}