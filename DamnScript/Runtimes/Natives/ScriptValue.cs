using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
    private ScriptValue(void* value) => pointerValue = value;

    public static ScriptValue FromReference<T>(T value) where T : class
    {
        var ptr = (void**)Unsafe.AsPointer(ref value);
        return new ScriptValue(*ptr);
    }

    public T GetReference<T>() where T : class
    {
        var ptr = pointerValue;
        return Unsafe.AsRef<T>(&ptr);
    }
    
    public static implicit operator ScriptValue(long value) => new(value);
}