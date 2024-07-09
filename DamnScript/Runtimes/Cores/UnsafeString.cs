using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Natives;

#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
using PinHandle = DamnScript.Runtimes.Cores.DSObjectPin;
#endif

namespace DamnScript.Runtimes.Cores
{
    public unsafe struct UnsafeStringPair
    {
        public readonly int hash;
        public readonly UnsafeString* value;
    
        public UnsafeStringPair(int hash, UnsafeString* value)
        {
            this.hash = hash;
            this.value = value;
        }
    }

    public unsafe struct UnsafeString : IDisposable
    {
        private static string _buffer;
        private static GCHandle _gcHandleBuffer;
        private static UnmanagedString* _bufferPtr;

        public ref char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref data[index];
        }

        public int length;
        public fixed char data[1];

        public UnsafeString() => throw new Exception("UnsafeString cannot be created without allocation.");

        public static UnsafeString* Alloc(int length)
        {
            var str = (UnsafeString*)UnsafeUtilities.Alloc((length + 1) * sizeof(char) + sizeof(int));
            str->length = length;
            str->data[length - 1] = '\0';
            return str;
        }
    
        public static UnsafeString* Alloc(string value)
        {
            var length = value.Length;
            var str = Alloc(length);
            fixed (char* ptr = value)
                UnsafeUtilities.Memcpy(ptr, str->data, length * sizeof(char));
            return str;
        }
    
        public static UnsafeString* Alloc(string value, int start, int length)
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

        public string ToTempStringNonAlloc()
        {
            if (_bufferPtr == null)
            {
                _buffer = new string('\0', 1024);
                _gcHandleBuffer = UnsafeUtilities.PinAsDotNet(_buffer);
                _bufferPtr = (UnmanagedString*)_gcHandleBuffer.AddrOfPinnedObject();
            }
        
            _bufferPtr->length = length;
            fixed (char* ptr = data)
                UnsafeUtilities.Memcpy(ptr, _bufferPtr->data, length * sizeof(char));
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
    
        public void Dispose()
        {
            fixed (UnsafeString* ptr = &this)
                UnsafeUtilities.Free(ptr);
        
            this = default;
        }

        public static void ReleaseTempStringNonAlloc()
        {
            _bufferPtr = null;
            _gcHandleBuffer.Free();
            _bufferPtr = null;
        }

        private struct UnmanagedString
        {
            public void* methodVTable;
#if MONO
        public void* syncRoot;
#endif
            public int length;
            public fixed char data[1];

            public UnmanagedString(void* methodVTable, int length, char* data)
            {
                this.methodVTable = methodVTable;
                this.length = length;
                fixed (char* ptr = this.data)
                    UnsafeUtilities.Memcpy(data, ptr, length * sizeof(char));
            }
        }
    }
}