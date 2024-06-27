using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct NativeList<T> where T : unmanaged
{
    public bool IsInitialized => Data != null;
    public int Count { get; private set; }
    public int Capacity { get; private set; }
    public T* Data { get; private set; }

    public NativeList(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException("Capacity must be greater than zero.");
        
        Data = (T*)UnsafeUtilities.Alloc(sizeof(T) * capacity);
        if (Data == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeList.");
        Count = 0;
        Capacity = capacity;
    }

    public void Add(T value)
    {
        if (Data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (Count == Capacity)
        {
            Capacity *= 2;
            var newData = (T*)UnsafeUtilities.Realloc(Data, Capacity * sizeof(T));
            if (newData == null)
                throw new OutOfMemoryException("Failed to reallocate memory for NativeList.");
            
            Data = newData;
        }
        Data[Count++] = value;
    }
    
    public void Remove(T value)
    {
        if (Data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        for (var i = 0; i < Count; i++)
        {
            if (!UnsafeUtilities.Memcmp(Data, &value))
                continue;
            
            RemoveAt(i);
            return;
        }
    }
    
    public void RemoveAt(int index)
    {
        if (Data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index is out of range.");
        
        UnsafeUtilities.Memcpy(Data + index, Data + index + 1, (Count - index - 1) * sizeof(T));
        Count--;
    }
    
    public void Clear()
    {
        if (Data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        Count = 0;
    }
    
    public NativeArray<T> ToArrayAlloc()
    {
        if (Data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        var copiedData = (T*)UnsafeUtilities.Alloc(sizeof(T) * Count);
        if (copiedData == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeArray.");
        UnsafeUtilities.Memcpy(copiedData, Data, Count * sizeof(T));
        return new NativeArray<T>(Count, copiedData);
    }
    
    public void Dispose()
    {
        if (Data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        UnsafeUtilities.Free(Data);
        this = default;
    }
}