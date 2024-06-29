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
    
    public ScriptValue(long value) => longValue = value;
    public ScriptValue(void* value) => pointerValue = value;

    public static ScriptValue FromReference<T>(T value) where T : class => new(UnsafeUtilities.ReferenceToPointer(value));
    
    public static ScriptValue FromStructAlloc<T>(T value) where T : unmanaged
    {
        var allocate = UnsafeUtilities.Alloc(sizeof(T));
        UnsafeUtilities.Memcpy(allocate, Unsafe.AsPointer(ref value), sizeof(T));
        return new ScriptValue(allocate);
    }

    public T GetReference<T>() where T : class => UnsafeUtilities.PointerToReference<T>(pointerValue);
    
    public T GetStruct<T>(bool freeAfterReturn = true) where T : unmanaged
    {
        var value = *(T*)pointerValue;
        if (freeAfterReturn)
            UnsafeUtilities.Free(pointerValue);
        return value;
    }
    
    public UnsafeString* GetUnsafeString() => (UnsafeString*)pointerValue;
    
    public static implicit operator ScriptValue(long value) => new(value);
}