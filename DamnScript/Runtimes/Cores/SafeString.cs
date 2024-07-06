using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public unsafe struct SafeString : IDisposable
{
    [FieldOffset(0)] public readonly string safeValue;
    [FieldOffset(0)] public readonly UnsafeString* unsafeValue;
    [FieldOffset(8)] public GCHandle gcHandle;

    public SafeString(string value)
    {
        safeValue = value;
        gcHandle = GCHandle.Alloc(value);
    }
    
    public SafeString(UnsafeString* value) => unsafeValue = value;
    
    public String32 ToString32() => gcHandle.IsAllocated 
        ? new String32(safeValue) 
        : new String32(unsafeValue->data, unsafeValue->length);
    
    public UnsafeString* ToUnsafeString() => gcHandle.IsAllocated 
        ? UnsafeString.Alloc(safeValue) 
        : unsafeValue;
    
    public override string ToString() => gcHandle.IsAllocated 
        ? safeValue 
        : unsafeValue->ToString();

    public void Dispose()
    {
        if (gcHandle.IsAllocated)
            gcHandle.Free();
        this = default;
    }

    public static implicit operator SafeString(string value) => new(value);
    public static implicit operator SafeString(UnsafeString* value) => new(value);
}