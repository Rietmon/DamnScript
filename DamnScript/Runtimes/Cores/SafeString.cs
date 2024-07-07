using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

[StructLayout(LayoutKind.Explicit, Size = 12)]
public unsafe struct SafeString : IDisposable
{
    public bool IsSafe => safeStringType == SafeStringType.Safe;
    
    [FieldOffset(0)] public SafeStringType safeStringType;
    [FieldOffset(4)] public GCHandle safeValue;
    [FieldOffset(4)] public UnsafeString* unsafeValue;

    public SafeString(string value)
    {
        safeStringType = SafeStringType.Safe;
        safeValue = UnsafeUtilities.Pin(value);
    }

    public SafeString(UnsafeString* value)
    {
        safeStringType = SafeStringType.Unsafe;
        unsafeValue = value;
    }
    
    public String32 ToString32() => IsSafe
        ? new String32((string)safeValue.Target) 
        : new String32(unsafeValue->data, unsafeValue->length);
    
    public UnsafeString* ToUnsafeString() => IsSafe
        ? UnsafeString.Alloc((string)safeValue.Target) 
        : unsafeValue;
    
    public override string ToString() => IsSafe
        ? (string)safeValue.Target 
        : unsafeValue->ToString();

    public void Dispose()
    {
        if (IsSafe)
            safeValue.Free();
        this = default;
    }

    public static implicit operator SafeString(string value) => new(value);
    public static implicit operator SafeString(UnsafeString* value) => new(value);
    
    public enum SafeStringType
    {
        Invalid,
        Safe,
        Unsafe
    }
}