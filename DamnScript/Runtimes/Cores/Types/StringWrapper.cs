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
    [StructLayout(LayoutKind.Explicit, Size = Size)]
    public unsafe struct StringWrapper : IDisposable
    {
        public const int Size = UnsafeUtilities.PointerSize * 2;
        
        public bool IsManaged => type is SafeStringType.Managed or SafeStringType.ManagedAlreadyPinned;
    
        [FieldOffset(0)] public SafeStringType type;
        [FieldOffset(UnsafeUtilities.PointerSize)] public ObjectPin safeValue;
        [FieldOffset(UnsafeUtilities.PointerSize)] public NativeString* unsafeValue;

        public StringWrapper(string value) : this()
        {
            type = SafeStringType.Managed;
            safeValue = UnsafeUtilities.Pin(value);
        }

        public StringWrapper(ObjectPin value) : this()
        {
            type = SafeStringType.ManagedAlreadyPinned;
            safeValue = value;
        }

        public StringWrapper(NativeString* value) : this()
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

        public static implicit operator StringWrapper(string value) => new(value);
        public static implicit operator StringWrapper(ObjectPin value) => new(value);
        public static implicit operator StringWrapper(NativeString* value) => new(value);
    
        public enum SafeStringType
        {
            Invalid,
            Managed,
            ManagedAlreadyPinned,
            Unmanaged
        }
    }
}