#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
using PinHandle = DamnScript.Runtimes.Cores.Pins.DSObjectPin;
#endif
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores.Strings
{
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public unsafe struct SafeString : IDisposable
    {
        public bool IsManaged => type == SafeStringType.Managed;
    
        [FieldOffset(0)] public SafeStringType type;
        [FieldOffset(4)] public PinHandle safeValue;
        [FieldOffset(4)] public UnsafeString* unsafeValue;

        public SafeString(string value)
        {
            type = SafeStringType.Managed;
            safeValue = UnsafeUtilities.Pin(value);
        }

        public SafeString(UnsafeString* value)
        {
            type = SafeStringType.Unmanaged;
            unsafeValue = value;
        }
    
        public String32 ToString32() => IsManaged
            ? new String32((string)safeValue.Target) 
            : new String32(unsafeValue->data, unsafeValue->length);
    
        public UnsafeString* ToUnsafeString() => IsManaged
            ? UnsafeString.Alloc((string)safeValue.Target) 
            : unsafeValue;
    
        public override string ToString() => IsManaged
            ? (string)safeValue.Target 
            : unsafeValue->ToString();

        public void Dispose()
        {
            if (IsManaged)
                safeValue.Free();
            this = default;
        }

        public static implicit operator SafeString(string value) => new(value);
        public static implicit operator SafeString(UnsafeString* value) => new(value);
    
        public enum SafeStringType
        {
            Invalid,
            Managed,
            Unmanaged
        }
    }
}