using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Collections
{
    public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
    {
        public T* Begin { get; }
        public int Length { get; }

        public T* Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => End - 1;
        }

        public T* End
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Begin + Length;
        }

        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Begin != null;
        }

        public ref T this[Index index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var i = index.GetOffset(Length);
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
                if (i < 0 || i >= Length)
                    throw new IndexOutOfRangeException();
#endif
                return ref Begin[i];
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
    
        public NativeList<T> ToListAlloc()
        {
            AssertIfNotInitialized();
            
            var list = new NativeList<T>(Length);
            list.AddRange(Begin, Length);
            return list;
        }
        
        public void FreePointersInside()
        {
            AssertIfNotInitialized();
            
            if (sizeof(T) != UnsafeUtilities.PointerSize)
                throw new InvalidOperationException("FreePointersInside is only available for pointer types.");
            
            var begin = (IntPtr**)Begin;
            for (var i = 0; i < Length; i++)
            {
                UnsafeUtilities.Free(*begin);
                Begin[i] = default;
                begin++;
            }
        }
    
        public void Dispose()
        {
            AssertIfNotInitialized();
            
            UnsafeUtilities.Free(Begin);
            this = default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AssertIfNotInitialized()
        {
            if (!IsValid)
                throw new NullReferenceException("NativeArray is not initialized.");
        }

        public static void ReAlloc(NativeArray<T>* ptr, int newSize)
        {
            ptr->AssertIfNotInitialized();
            
            var newBegin = (T*)UnsafeUtilities.ReAlloc(ptr->Begin, sizeof(T) * newSize);
            *ptr = new NativeArray<T>(newSize, newBegin);
        }
    }
}