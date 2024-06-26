using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct NativeList<T> where T : unmanaged
{
    public bool IsInitialized => _data != null;
    public int Count { get; private set; }
    public int Capacity { get; private set; }

    public T this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }  
    
    private T* _data;

    public NativeList(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException("Capacity must be greater than zero.");
        
        _data = (T*)UnsafeUtilities.Alloc(sizeof(T) * capacity);
        if (_data == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeList.");
        Count = 0;
        Capacity = capacity;
    }

    public void Add(T value)
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (Count == Capacity)
        {
            Capacity *= 2;
            var newData = (T*)UnsafeUtilities.Realloc(_data, Capacity * sizeof(T));
            if (newData == null)
                throw new OutOfMemoryException("Failed to reallocate memory for NativeList.");
            
            _data = newData;
        }
        _data[Count++] = value;
    }
    
    public void Remove(T value)
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        for (var i = 0; i < Count; i++)
        {
            if (!UnsafeUtilities.Memcmp(_data, &value))
                continue;
            
            RemoveAt(i);
            return;
        }
    }
    
    public void RemoveAt(int index)
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (index < 0 || index >= Count)
            throw new IndexOutOfRangeException("Index is out of range.");
        
        UnsafeUtilities.Memcpy(_data + index, _data + index + 1, (Count - index - 1) * sizeof(T));
        Count--;
    }
    
    public void Clear()
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        Count = 0;
    }
    
    public NativeArray<T> ToArrayAlloc()
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        var copiedData = (T*)UnsafeUtilities.Alloc(sizeof(T) * Count);
        if (copiedData == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeArray.");
        UnsafeUtilities.Memcpy(copiedData, _data, Count * sizeof(T));
        return new NativeArray<T>(Count, copiedData);
    }
    
    public void Dispose()
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        UnsafeUtilities.Free(_data);
        this = default;
    }
}