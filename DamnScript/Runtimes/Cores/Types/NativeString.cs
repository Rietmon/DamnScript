using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores.Pins;

namespace DamnScript.Runtimes.Cores.Types
{
    public readonly unsafe struct NativeStringPtr
    {
        public readonly NativeString* value;
    
        public NativeStringPtr(NativeString* value) => this.value = value;

        public static implicit operator NativeStringPtr(NativeString* value) => new(value);
    
        public static implicit operator NativeString*(NativeStringPtr ptr) => ptr.value;
    }

    /// <summary>
    /// String implementation without managed allocation.
    /// Can be used for fast string manipulation.
    /// Might be converted to managed string.
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public unsafe struct NativeString
    {
        private static string _buffer;
        private static ObjectPin _gcHandleBuffer;

        public ref char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref data[index];
        }

        public int length;
        public fixed char data[1];

        public static NativeString* Alloc(int length)
        {
            var str = (NativeString*)UnsafeUtilities.Alloc((length + 1) * sizeof(char) + sizeof(int));
            str->length = length;
            str->data[length - 1] = '\0';
            return str;
        }
    
        public static NativeString* Alloc(string value)
        {
            var length = value.Length;
            var str = Alloc(length);
            fixed (char* ptr = value)
                UnsafeUtilities.Memcpy(ptr, str->data, length * sizeof(char));
            return str;
        }
    
        public static NativeString* Alloc(string value, int start, int length)
        {
            var str = Alloc(length);
            fixed (char* ptr = value)
                UnsafeUtilities.Memcpy(ptr + start, str->data, length * sizeof(char));
            return str;
        }
    
        public override string ToString()
        {
            fixed (char* ptr = data)
                return new string(ptr, 0, length);
        }
        
        public String32 ToString32()
        {
            if (length > String32.Length)
                throw new ArgumentException("String length must be less than or equal to 32.");
        
            fixed (char* ptr = data)
                return new String32(ptr, length);
        }

        public string ToTempStringNonAlloc()
        {
            if (_gcHandleBuffer == default)
            {
                _buffer = new string('\0', 1024);
                _gcHandleBuffer = UnsafeUtilities.Pin(_buffer);
            }

            var stringPtr = (UnmanagedString*)_gcHandleBuffer.GetAddress();
            stringPtr->length = length;
            fixed (char* ptr = data)
                UnsafeUtilities.Memcpy(ptr, stringPtr->data, length * sizeof(char));
            return _buffer;
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            var hash = 0;
            for (var i = 0; i < length; i++)
                hash = 31 * hash + data[i];
            return hash;
        }

        public static void ReleaseTempStringNonAlloc()
        {
            _buffer = null;
            _gcHandleBuffer.Free();
        }

        [DebuggerDisplay("{ToString()}")]
        public struct UnmanagedString
        {
            public void* methodVTable;
#if DAMN_SCRIPT_ENBALE_MONO || UNITY_5_3_OR_NEWER
            public void* syncRoot;
#endif
            public int length;
            public fixed char data[1];

            public UnmanagedString(void* methodVTable, int length, char* data) : this()
            {
                this.methodVTable = methodVTable;
                this.length = length;
                fixed (char* ptr = this.data)
                    UnsafeUtilities.Memcpy(data, ptr, length * sizeof(char));
            }

            public override string ToString()
            {
                fixed (char* ptr = data)
                    return new string(ptr, 0, length);
            }
        }
    }
}