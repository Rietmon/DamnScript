using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
using PinHandle = DamnScript.Runtimes.Cores.DSObjectPin;
#endif

namespace DamnScript.Runtimes.Cores
{
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public unsafe struct SafeString : IDisposable
    {
        public bool IsSafe => type == SafeStringType.Safe;
    
        [FieldOffset(0)] public SafeStringType type;
        [FieldOffset(4)] public PinHandle safeValue;
        [FieldOffset(4)] public UnsafeString* unsafeValue;

        public SafeString(string value)
        {
            type = SafeStringType.Safe;
            safeValue = UnsafeUtilities.Pin(value);
        }

        public SafeString(UnsafeString* value)
        {
            type = SafeStringType.Unsafe;
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
}