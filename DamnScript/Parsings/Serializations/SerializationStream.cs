using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

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
                UnsafeUtilities.Free(start);
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
                UnsafeUtilities.Free(start);
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
                UnsafeUtilities.Free(start);
                start = newPtr;
            }

            UnsafeUtilities.Memcpy(ptr, start + length, count);
            length += count;
        }

        public void WriteUnsafeStringArray(NativeArray<UnsafeStringPtr> array)
        {
            Write(array.Length);
            var begin = array.First;
            var end = array.End;
            while (begin < end)
            {
                var str = begin->value;
                Write(str->length);
                CustomWrite(str->data, str->length);
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

        public NativeArray<UnsafeStringPtr> ReadUnsafeStringArray()
        {
            var arrayLength = Read<int>();
            if (arrayLength == 0)
                throw new Exception("Invalid array length!");
            
            var result = new NativeArray<UnsafeStringPtr>(arrayLength);
            for (var i = 0; i < arrayLength; i++)
            {
                var strLength = Read<int>();
                var str = UnsafeString.Alloc(strLength);
                CustomRead(str, strLength);
                result.First[i] = new UnsafeStringPtr(str);
            }
            
            return result;
        }

        public void Dispose()
        {
            UnsafeUtilities.Free(start);
            this = default;
        }
    }
}