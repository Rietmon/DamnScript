using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct NativeList<T> where T : unmanaged
{
    public bool IsInitialized => _data != null;
    public int Length { get; private set; }
    public int Capacity { get; private set; }

    public T this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }  
    
    private T* _data;

    public NativeList(int capacity)
    {
        _data = (T*)UnsafeUtilities.Alloc(sizeof(T) * capacity);
        if (_data == null)
            throw new OutOfMemoryException("Failed to allocate memory for NativeList.");
        Length = 0;
        Capacity = capacity;
    }

    public void Add(T value)
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        if (Length == Capacity)
        {
            Capacity *= 2;
            var newData = (T*)UnsafeUtilities.Realloc(_data, Capacity * sizeof(T));
            if (newData == null)
                throw new OutOfMemoryException("Failed to reallocate memory for NativeList.");
            
            _data = newData;
        }
        _data[Length++] = value;
    }
    
    public void Remove(T value)
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        for (var i = 0; i < Length; i++)
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
        
        if (index < 0 || index >= Length)
            throw new IndexOutOfRangeException("Index is out of range.");
        
        UnsafeUtilities.Memcpy(_data + index, _data + index + 1, (Length - index - 1) * sizeof(T));
        Length--;
    }
    
    public void Clear()
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        Length = 0;
    }
    
    public void Dispose()
    {
        if (_data == null)
            throw new NullReferenceException("NativeList is not initialized.");
        
        UnsafeUtilities.Free(_data);
        this = default;
    }
}