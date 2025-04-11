using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Runtimes.Cores.Types
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
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref Begin[index];
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