using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Collections
{
    public unsafe struct NativeList<T> where T : unmanaged
    {
        public int Count { get; private set; }
        public int Capacity { get; private set; }
        public T* Begin { get; private set; }

        public T* Last
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => End - 1;
        }

        public T* End
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Begin + Count;
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
                var i = index.GetOffset(Count);
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
                if (i < 0 || i >= Count)
                    throw new IndexOutOfRangeException();
#endif
                
                return ref Begin[i];
            }
        }

        public NativeList(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");
        
            Begin = (T*)UnsafeUtilities.Alloc(sizeof(T) * capacity);
            Count = 0;
            Capacity = capacity;
        }

        public void Add(T value)
        {
            AssertIfNotInitialized();
        
            if (Count == Capacity)
            {
                Capacity *= 2;
                Begin = (T*)UnsafeUtilities.ReAlloc(Begin, Capacity * sizeof(T));
            }
            Begin[Count++] = value;
        }
    
        public void AddRange(T* values, int length)
        {
            AssertIfNotInitialized();
        
            if (Count + length > Capacity)
            {
                Capacity = Count + length;
                Begin = (T*)UnsafeUtilities.ReAlloc(Begin, Capacity * sizeof(T));
            }
            UnsafeUtilities.Memcpy(values, Begin + Count, length * sizeof(T));
            Count += length;
        }
    
        public bool Remove(T value)
        {
            AssertIfNotInitialized();
        
            var begin = Begin;
            var end = End;
            var i = 0;
            while (begin < end)
            {
                if (UnsafeUtilities.Memcmp(begin, &value))
                {
                    RemoveAt(i);
                    return true;
                }
                begin++;
                i++;
            }
        
            return false;
        }
    
        public void RemoveAt(int index)
        {
            AssertIfNotInitialized();
        
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException("Index is out of range.");
        
            UnsafeUtilities.Memcpy(Begin + index + 1, Begin + index, (Count - index - 1) * sizeof(T));
            Count--;
        }
        
        public int IndexOf(T value)
        {
            AssertIfNotInitialized();
        
            var begin = Begin;
            var end = End;
            var i = 0;
            while (begin < end)
            {
                if (UnsafeUtilities.Memcmp(begin, &value))
                    return i;
                begin++;
                i++;
            }
        
            return -1;
        }
        
        public int IndexOf(Func<T, bool> predicate)
        {
            AssertIfNotInitialized();
        
            var begin = Begin;
            var end = End;
            var i = 0;
            while (begin < end)
            {
                if (predicate(*begin))
                    return i;
                begin++;
                i++;
            }
        
            return -1;
        }
    
        public void Clear()
        {
            AssertIfNotInitialized();
        
            Count = 0;
        }
    
        public NativeArray<T> ToArrayAlloc()
        {
            AssertIfNotInitialized();
        
            var copiedData = (T*)UnsafeUtilities.Alloc(sizeof(T) * Count);
            UnsafeUtilities.Memcpy(Begin, copiedData, Count * sizeof(T));
            return new NativeArray<T>(Count, copiedData);
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
    }
}