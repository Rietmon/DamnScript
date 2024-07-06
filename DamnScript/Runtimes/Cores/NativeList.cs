using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct NativeList<T> where T : unmanaged
{
    public bool IsInitialized => Begin != null;
    public int Count { get; private set; }
    public int Capacity { get; private set; }
    public T* Begin { get; private set; }
    public T* End => Begin + Count;

    public ref T this[int index] => ref Begin[index];

    public NativeList(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be greater than 0.");
        
        Begin = (T*)UnsafeUtilities.Alloc(sizeof(T) * capacity);
        if (Begin == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeList.");
        Count = 0;
        Capacity = capacity;
    }

    public void Add(T value)
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (Count == Capacity)
        {
            Capacity *= 2;
            var newData = (T*)UnsafeUtilities.ReAlloc(Begin, Capacity * sizeof(T));
            if (newData == null)
                throw new OutOfMemoryException("Failed to reallocate memory for NativeList.");
            
            Begin = newData;
        }
        Begin[Count++] = value;
    }
    
    public void AddRange(T* values, int length)
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (Count + length > Capacity)
        {
            Capacity = Count + length;
            var newData = (T*)UnsafeUtilities.ReAlloc(Begin, Capacity * sizeof(T));
            if (newData == null)
                throw new OutOfMemoryException("Failed to reallocate memory for NativeList.");
            
            Begin = newData;
        }
        UnsafeUtilities.Memcpy(Begin + Count, values, length * sizeof(T));
        Count += length;
    }
    
    public void Remove(T value)
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        for (var i = 0; i < Count; i++)
        {
            if (!UnsafeUtilities.Memcmp(Begin, &value))
                continue;
            
            RemoveAt(i);
            return;
        }
    }
    
    public void RemoveAt(int index)
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index is out of range.");
        
        UnsafeUtilities.Memcpy(Begin + index, Begin + index + 1, (Count - index - 1) * sizeof(T));
        Count--;
    }
    
    public void Clear()
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        Count = 0;
    }
    
    public NativeArray<T> ToArrayAlloc()
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        var copiedData = (T*)UnsafeUtilities.Alloc(sizeof(T) * Count);
        if (copiedData == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeArray.");
        UnsafeUtilities.Memcpy(copiedData, Begin, Count * sizeof(T));
        return new NativeArray<T>(Count, copiedData);
    }
    
    public void Dispose()
    {
        if (Begin == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        UnsafeUtilities.Free(Begin);
        this = default;
    }
}