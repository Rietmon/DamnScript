using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
{
    public int Length { get; private set; }
    
    public T* Data { get; private set; }
    
    public NativeArray(int length)
    {
        Data = (T*)UnsafeUtilities.Alloc(sizeof(T) * length);
    }
    
    public NativeArray(int length, T* data)
    {
        Length = length;
        Data = data;
    }
    
    public T* Begin => Data;
    
    public T* End => Data + Length;
    
    public void Dispose()
    {
        UnsafeUtilities.Free(Data);
        this = default;
    }
}