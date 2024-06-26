﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
{
    public int Length { get; private set; }
    
    private T* _data;
    
    public NativeArray(int length)
    {
        _data = (T*)UnsafeUtilities.Alloc(sizeof(T) * length);
    }
    
    public NativeArray(int length, T* data)
    {
        Length = length;
        _data = data;
    }
    
    public T this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }
    
    public void Dispose()
    {
        UnsafeUtilities.Free(_data);
        this = default;
    }
}