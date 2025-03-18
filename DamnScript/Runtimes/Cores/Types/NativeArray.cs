using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Metadatas;

namespace DamnScript.Runtimes.Cores.Types
{
    public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
    {
        public int Length { get; }
    
        public T* Begin { get; }
    
        public T* Last => End - 1;
        
        public T* End => Begin + Length;

        public bool IsValid => Begin != null;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref Begin[index];
        }
    
        public NativeArray(int length)
        {
            Length = length;
            Begin = (T*)UnsafeUtilities.Alloc(sizeof(T) * length);
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
    }
    
    public static unsafe class NativeArrayExtensions
    {
        public static NativeArray<T> ReAlloc<T>(this NativeArray<T> array, int newLength) where T : unmanaged
        {
            UnsafeUtilities.ReAlloc(array.Begin, sizeof(T) * newLength);
            return new NativeArray<T>(newLength, array.Begin);
        }
    }
}