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
    [StructLayout(LayoutKind.Sequential, Size = 12)]
    public unsafe struct StringWrapper
    {
        public bool IsManaged => type == ConstStringType.Managed;
    
        public ConstStringType type;
        public string managedStringValue;
        public UnsafeString* unmanagedStringValue;

        public StringWrapper(string value) : this()
        {
            type = ConstStringType.Managed;
            managedStringValue = value;
        }

        public StringWrapper(UnsafeString* value) : this()
        {
            type = ConstStringType.Unmanaged;
            unmanagedStringValue = value;
        }
    
        public String32 ToString32() => IsManaged
            ? new String32(managedStringValue) 
            : new String32(unmanagedStringValue->data, unmanagedStringValue->length);
    
        public UnsafeString* ToUnsafeString() => IsManaged
            ? UnsafeString.Alloc(managedStringValue) 
            : unmanagedStringValue;
    
        public override string ToString() => IsManaged
            ? managedStringValue 
            : unmanagedStringValue->ToString();

        public static implicit operator StringWrapper(string value) => new(value);
        public static implicit operator StringWrapper(UnsafeString* value) => new(value);
    
        public enum ConstStringType
        {
            Invalid,
            Managed,
            Unmanaged
        }
    }
}