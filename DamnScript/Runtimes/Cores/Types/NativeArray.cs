using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Types
{
    public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
    {
        public int Length { get; }
        
        public T* Begin => First - 1;
    
        public T* First { get; }
    
        public T* Last => First + Length - 1;
        
        public T* End => First + Length;

        public bool IsValid => First != null;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref First[index];
        }
    
        public NativeArray(int length)
        {
            Length = length;
            First = (T*)UnsafeUtilities.Alloc(sizeof(T) * length);
        }
    
        public NativeArray(int length, T* first)
        {
            Length = length;
            First = first;
        }
    
        public NativeList<T> ToListAlloc()
        {
            var list = new NativeList<T>(Length);
            list.AddRange(First, Length);
            return list;
        }
    
        public void Dispose()
        {
            UnsafeUtilities.Free(First);
            this = default;
        }
    }
}