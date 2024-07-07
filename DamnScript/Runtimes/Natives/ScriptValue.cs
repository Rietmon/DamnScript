using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Natives;

[StructLayout(LayoutKind.Explicit)]
public unsafe struct ScriptValue
{
    [FieldOffset(0)] public bool boolValue;
    [FieldOffset(0)] public byte byteValue;
    [FieldOffset(0)] public short shortValue;
    [FieldOffset(0)] public int intValue;
    [FieldOffset(0)] public long longValue;
    [FieldOffset(0)] public float floatValue;
    [FieldOffset(0)] public double doubleValue;
    [FieldOffset(0)] public char charValue;
    [FieldOffset(0)] public void* pointerValue;
    [FieldOffset(0)] public GCHandle safeValue;
    
    public ScriptValue(bool value) => boolValue = value;
    public ScriptValue(byte value) => byteValue = value;
    public ScriptValue(short value) => shortValue = value;
    public ScriptValue(int value) => intValue = value;
    public ScriptValue(long value) => longValue = value;
    public ScriptValue(float value) => floatValue = value;
    public ScriptValue(double value) => doubleValue = value;
    public ScriptValue(char value) => charValue = value;
    public ScriptValue(void* value) => pointerValue = value;
    public ScriptValue(GCHandle value) => safeValue = value;

    public static ScriptValue FromReferencePin<T>(T value) where T : class => new(GCHandle.Alloc(value, GCHandleType.Pinned));
    
    public static ScriptValue FromReference<T>(T value) where T : class => new(UnsafeUtilities.ReferenceToPointer(value));
    
    public static ScriptValue FromStructAlloc<T>(T value) where T : unmanaged
    {
        var allocate = UnsafeUtilities.Alloc(sizeof(T));
        UnsafeUtilities.Memcpy(allocate, Unsafe.AsPointer(ref value), sizeof(T));
        return new ScriptValue(allocate);
    }

    public T GetReferencePin<T>(bool freeBeforeReturn = true) where T : class
    {
        var value = (T)safeValue.Target;
        if (freeBeforeReturn)
            safeValue.Free();
        return value;
    }
    
    public T GetReference<T>() where T : class => UnsafeUtilities.PointerToReference<T>(pointerValue);
    
    public T GetStruct<T>(bool freeBeforeReturn = true) where T : unmanaged
    {
        var value = *(T*)pointerValue;
        if (freeBeforeReturn)
            UnsafeUtilities.Free(pointerValue);
        return value;
    }
    
    public UnsafeString* GetUnsafeString() => (UnsafeString*)pointerValue;
    
    public SafeString GetSafeString() => new(GetUnsafeString()->ToString());
    
    public static implicit operator ScriptValue(bool value) => new(value);
    public static implicit operator ScriptValue(byte value) => new(value);
    public static implicit operator ScriptValue(short value) => new(value);
    public static implicit operator ScriptValue(int value) => new(value);
    public static implicit operator ScriptValue(long value) => new(value);
    public static implicit operator ScriptValue(float value) => new(value);
    public static implicit operator ScriptValue(double value) => new(value);
    public static implicit operator ScriptValue(char value) => new(value);
    public static implicit operator ScriptValue(void* value) => new(value);
    public static implicit operator ScriptValue(GCHandle value) => new(value);
}