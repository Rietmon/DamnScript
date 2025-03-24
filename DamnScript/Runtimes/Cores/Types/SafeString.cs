using System;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Pins;

namespace DamnScript.Runtimes.Cores.Types
{
    /// <summary>
    /// Wrapper for string and NativeString.
    /// It can be created from string or NativeString.
    /// If provided string, it will be pinned, then you should implicitly Dispose it.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public unsafe struct SafeString : IDisposable
    {
        public bool IsManaged => type is SafeStringType.Managed or SafeStringType.ManagedAlreadyPinned;
    
        [FieldOffset(0)] public SafeStringType type;
        [FieldOffset(4)] public ObjectPin safeValue;
        [FieldOffset(4)] public NativeString* unsafeValue;

        public SafeString(string value) : this()
        {
            type = SafeStringType.Managed;
            safeValue = UnsafeUtilities.Pin(value);
        }

        public SafeString(ObjectPin value) : this()
        {
            type = SafeStringType.ManagedAlreadyPinned;
            safeValue = value;
        }

        public SafeString(NativeString* value) : this()
        {
            type = SafeStringType.Unmanaged;
            unsafeValue = value;
        }
    
        public String32 ToString32() => IsManaged
            ? new String32((string)safeValue.Target) 
            : new String32(unsafeValue->data, unsafeValue->length);
    
        public NativeString* ToNativeString() => IsManaged
            ? NativeString.Alloc((string)safeValue.Target) 
            : unsafeValue;
    
        public override string ToString() => IsManaged
            ? (string)safeValue.Target 
            : unsafeValue->ToString();

        public void Dispose()
        {
            if (type is SafeStringType.Managed)
                safeValue.Free();
            this = default;
        }

        public static implicit operator SafeString(string value) => new(value);
        public static implicit operator SafeString(ObjectPin value) => new(value);
        public static implicit operator SafeString(NativeString* value) => new(value);
    
        public enum SafeStringType
        {
            Invalid,
            Managed,
            ManagedAlreadyPinned,
            Unmanaged
        }
    }
}