using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Natives;

namespace DamnScript.Runtimes.Cores;

public unsafe struct UnsafeString : IDisposable
{
    private static readonly string buffer = new(char.MinValue, 1024);
    private static readonly UnmanagedString* bufferPtr = (UnmanagedString*)ScriptValue.FromReference(buffer).pointerValue;

    public int length;
    
    public fixed char data[1];

    public static UnsafeString* Alloc(int length)
    {
        var str = (UnsafeString*)UnsafeUtilities.Alloc((length + 1) * sizeof(char) + sizeof(int));
        str->length = length;
        return str;
    }
    
    public static UnsafeString* Alloc(string value)
    {
        var length = value.Length;
        var str = Alloc(length);
        fixed (char* ptr = value)
            UnsafeUtilities.Memcpy(str->data, ptr, length * sizeof(char));
        return str;
    }
    
    public override string ToString()
    {
        fixed (char* ptr = data)
            return new string(ptr, 0, length);
    }

    public string ToTempStringNonAlloc()
    {
        bufferPtr->length = length;
        fixed (char* ptr = data)
            UnsafeUtilities.Memcpy(bufferPtr->data, ptr, length * sizeof(char));
        return buffer;
    }
    
    public void Dispose()
    {
        fixed (UnsafeString* ptr = &this)
            UnsafeUtilities.Free(ptr);
        
        this = default;
    }

    private struct UnmanagedString
    {
        public void* methodVTable;
#if MONO
        public void* syncRoot;
#endif
        public int length;
        public fixed char data[1];
    }
}