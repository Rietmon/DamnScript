using DamnScript.Runtimes.Cores;

namespace DamnScript.Parsings.Serializations;

internal unsafe struct SerializationStream : IDisposable
{
    public int Capacity => _capacity;
    public int Length => _length;
    
    private int _capacity;
    private int _length;
    private byte* _start;
    
    public SerializationStream(int capacity)
    {
        _start = (byte*)UnsafeUtilities.Alloc(capacity);
        _capacity = capacity;
        _length = 0;
    }
    
    public SerializationStream(byte* start, int capacity)
    {
        _start = start;
        _capacity = capacity;
        _length = 0;
    }
    
    public void Write<T>(T value) where T : unmanaged
    {
        var size = sizeof(T);
        if (_length + size > _capacity)
        {
            _capacity *= 2;
            var newPtr = (byte*)UnsafeUtilities.ReAlloc(_start, _capacity);
            UnsafeUtilities.Free(_start);
            _start = newPtr;
        }
        
        *(T*)_start = value;
        _length += size;
    }
    
    public void WriteBytes(byte* bytes, int count)
    {
        if (_length + count > _capacity)
        {
            _capacity *= 2;
            var newPtr = (byte*)UnsafeUtilities.ReAlloc(_start, _capacity);
            UnsafeUtilities.Free(_start);
            _start = newPtr;
        }
        
        UnsafeUtilities.Memcpy(bytes, _start + _length, count);
        _length += count;
    }
    
    public void CustomWrite<T>(T* ptr, int count) where T : unmanaged
    {
        if (_length + count > _capacity)
        {
            _capacity *= 2;
            var newPtr = (byte*)UnsafeUtilities.ReAlloc(_start, _capacity);
            UnsafeUtilities.Free(_start);
            _start = newPtr;
        }
        
        UnsafeUtilities.Memcpy(ptr, _start + _length, count);
        _length += count;
    }
    
    public T Read<T>() where T : unmanaged
    {
        var size = sizeof(T);
        if (_length + size > _capacity)
            return default;
        
        var value = *(T*)(_start + _length);
        _length += size;
        return value;
    }
    
    public byte* ReadBytes(int count)
    {
        if (_length + count > _capacity)
            return null;
        
        var ptr = _start + _length;
        _length += count;
        return ptr;
    }
    
    public void CustomRead<T>(T* ptr, int count) where T : unmanaged
    {
        if (_length + count > _capacity)
            return;
        
        UnsafeUtilities.Memcpy(_start + _length, ptr, count);
        _length += count;
    }

    public void Dispose()
    {
        UnsafeUtilities.Free(_start);
        this = default;
    }
}