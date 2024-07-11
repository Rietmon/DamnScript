#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
#endif
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores.Types
{
    /// <summary>
    /// Wrapper for string and UnsafeString.
    /// Represent const string which should be used only for arguments in methods.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public unsafe struct ConstString
    {
        public bool IsManaged => type == ConstStringType.Managed;
    
        [FieldOffset(0)] public ConstStringType type;
        [FieldOffset(4)] public void* managedStringValue;
        [FieldOffset(4)] public UnsafeString* unmanagedStringValue;

        public ConstString(string value)
        {
            type = ConstStringType.Managed;
            managedStringValue = UnsafeUtilities.ReferenceToPointer(value);
        }

        public ConstString(UnsafeString* value)
        {
            type = ConstStringType.Unmanaged;
            unmanagedStringValue = value;
        }
    
        public String32 ToString32() => IsManaged
            ? new String32(UnsafeUtilities.PointerToReference<string>(managedStringValue)) 
            : new String32(unmanagedStringValue->data, unmanagedStringValue->length);
    
        public UnsafeString* ToUnsafeString() => IsManaged
            ? UnsafeString.Alloc(UnsafeUtilities.PointerToReference<string>(managedStringValue)) 
            : unmanagedStringValue;
    
        public override string ToString() => IsManaged
            ? UnsafeUtilities.PointerToReference<string>(managedStringValue) 
            : unmanagedStringValue->ToString();

        public static implicit operator ConstString(string value) => new(value);
        public static implicit operator ConstString(UnsafeString* value) => new(value);
    
        public enum ConstStringType
        {
            Invalid,
            Managed,
            Unmanaged
        }
    }
}