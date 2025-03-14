using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores.Types
{
    /// <summary>
    /// Wrapper for string and UnsafeString.
    /// Represent const string which should be used only for arguments in methods.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct StringWrapper
    {
        public bool IsManaged => type == ConstStringType.Managed;
    
        public ConstStringType type;
        public void* stringPointer;
        
        public string AsManagedString => UnsafeUtilities.PointerToReference<string>(stringPointer);
        public UnsafeString* AsUnsafeString => (UnsafeString*)stringPointer;
        public String32 AsString32 => IsManaged ? new String32(AsManagedString) : AsUnsafeString->ToString32();

        public StringWrapper(string value) : this()
        {
            type = ConstStringType.Managed;
            stringPointer = UnsafeUtilities.ReferenceToPointer(value);
        }

        public StringWrapper(UnsafeString* value) : this()
        {
            type = ConstStringType.Unmanaged;
            stringPointer = value;
        }

        public static implicit operator StringWrapper(string value) => new(value);
        public static implicit operator StringWrapper(UnsafeString* value) => new(value);

        public static bool operator ==(StringWrapper l, StringWrapper r) =>
            l.type == r.type && l.stringPointer == r.stringPointer;
        public static bool operator !=(StringWrapper l, StringWrapper r) =>
            !(l.type == r.type && l.stringPointer == r.stringPointer);
    
        public enum ConstStringType
        {
            Invalid,
            Managed,
            Unmanaged
        }
    }
}