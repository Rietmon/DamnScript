using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

[StructLayout(LayoutKind.Explicit, Size = 8)]
public unsafe struct SafeString : IDisposable
{
    [FieldOffset(0)] public GCHandle safeValue;
    [FieldOffset(0)] public UnsafeString* unsafeValue;

    public SafeString(string value) => safeValue = GCHandle.Alloc(value);
    
    public SafeString(UnsafeString* value) => unsafeValue = value;
    
    public String32 ToString32() => safeValue.IsAllocated 
        ? new String32((string)safeValue.Target) 
        : new String32(unsafeValue->data, unsafeValue->length);
    
    public UnsafeString* ToUnsafeString() => safeValue.IsAllocated 
        ? UnsafeString.Alloc((string)safeValue.Target) 
        : unsafeValue;
    
    public override string ToString() => safeValue.IsAllocated 
        ? (string)safeValue.Target 
        : unsafeValue->ToString();

    public void Dispose()
    {
        if (safeValue.IsAllocated)
            safeValue.Free();
        this = default;
    }

    public static implicit operator SafeString(string value) => new(value);
    public static implicit operator SafeString(UnsafeString* value) => new(value);
}