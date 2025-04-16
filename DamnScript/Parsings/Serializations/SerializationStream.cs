using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;

namespace DamnScript.Parsings.Serializations
{
    public unsafe struct SerializationStream : IDisposable
    {
        public int capacity;
        public int length;
        public byte* start;

        public SerializationStream(int capacity)
        {
            start = (byte*)UnsafeUtilities.Alloc(capacity);
            this.capacity = capacity;
            length = 0;
        }

        public SerializationStream(byte* start, int capacity)
        {
            this.start = start;
            this.capacity = capacity;
            length = 0;
        }

        public void Write<T>(T value) where T : unmanaged
        {
            var size = sizeof(T);
            if (length + size > capacity)
            {
                capacity *= 2;
                var newPtr = (byte*)UnsafeUtilities.ReAlloc(start, capacity);
                start = newPtr;
            }

            *(T*)(start + length) = value;
            length += size;
        }

        public void WriteBytes(byte* bytes, int count)
        {
            if (length + count > capacity)
            {
                capacity *= 2;
                var newPtr = (byte*)UnsafeUtilities.ReAlloc(start, capacity);
                start = newPtr;
            }

            UnsafeUtilities.Memcpy(bytes, start + length, count);
            length += count;
        }

        public void CustomWrite<T>(T* ptr, int count) where T : unmanaged
        {
            if (length + count > capacity)
            {
                capacity *= 2;
                var newPtr = (byte*)UnsafeUtilities.ReAlloc(start, capacity);
                start = newPtr;
            }

            UnsafeUtilities.Memcpy(ptr, start + length, count);
            length += count;
        }

        public void WriteNativeStringArray(NativeArray<NativeStringPtr> array)
        {
            Write(array.Length);
            var begin = array.Begin;
            var end = array.End;
            while (begin < end)
            {
                var str = begin->value;
                Write(str->length);
                CustomWrite(str->data, str->length * sizeof(char));
                begin++;
            }
        }
        
        public T Read<T>() where T : unmanaged
        {
            var size = sizeof(T);
            if (length + size > capacity)
                return default;

            var value = *(T*)(start + length);
            length += size;
            return value;
        }

        public byte* ReadBytes(int count)
        {
            if (length + count > capacity)
                return null;

            var ptr = start + length;
            length += count;
            return ptr;
        }

        public void CustomRead<T>(T* ptr, int count) where T : unmanaged
        {
            if (length + count > capacity)
                return;

            UnsafeUtilities.Memcpy(start + length, ptr, count);
            length += count;
        }

        public NativeArray<NativeStringPtr> ReadNativeStringArray()
        {
            var arrayLength = Read<int>();
            
            var result = new NativeArray<NativeStringPtr>(arrayLength);
            for (var i = 0; i < arrayLength; i++)
            {
                var strLength = Read<int>();
                var str = NativeString.Alloc(strLength);
                CustomRead(str->data, strLength * sizeof(char));
                result.Begin[i] = new NativeStringPtr(str);
            }
            
            return result;
        }
        
        public Span<byte> AsSpan()
        {
            return new Span<byte>(start, length);
        }

        public void Dispose()
        {
            UnsafeUtilities.Free(start);
            this = default;
        }
    }
}