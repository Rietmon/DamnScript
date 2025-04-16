using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Collections
{
    public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
    {
        public T* Begin { get; }
        public int Length { get; }
    
        public T* Last => End - 1;
        
        public T* End => Begin + Length;

        public bool IsValid => Begin != null;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException();
#endif
                return ref Begin[index];
            }
        }
    
        public NativeArray(int length, bool clear = false)
        {
            Length = length;
            Begin = (T*)UnsafeUtilities.Alloc(sizeof(T) * length);
            if (clear)
                UnsafeUtilities.Memset(Begin, 0, sizeof(T) * length);
        }
    
        public NativeArray(int length, T* begin)
        {
            Length = length;
            Begin = begin;
        }

        public void CopyFrom(NativeArray<T> other)
        {
            if (Length < other.Length)
            {
                fixed (NativeArray<T>* p = &this)
                    ReAlloc(p, other.Length);
            }
            
            UnsafeUtilities.Memcpy(other.Begin, Begin, Length);
        }
    
        public NativeList<T> ToListAlloc()
        {
            var list = new NativeList<T>(Length);
            list.AddRange(Begin, Length);
            return list;
        }
    
        public void Dispose()
        {
            UnsafeUtilities.Free(Begin);
            this = default;
        }

        public static void ReAlloc(NativeArray<T>* ptr, int newSize)
        {
            var newBegin = (T*)UnsafeUtilities.ReAlloc(ptr->Begin, sizeof(T) * newSize);
            *ptr = new NativeArray<T>(newSize, newBegin);
        }
    }
}