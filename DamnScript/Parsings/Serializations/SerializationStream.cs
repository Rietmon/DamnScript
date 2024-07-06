using DamnScript.Runtimes.Cores;

namespace DamnScript.Parsings.Serializations;

internal unsafe struct SerializationStream
{
    private byte* _start;
    private int _length;

    private int _offset;
    
    public SerializationStream(int length)
    {
        _start = (byte*)UnsafeUtilities.Alloc(length);
        _length = length;
        _offset = 0;
    }
    
    public SerializationStream(byte* start, int length)
    {
        _start = start;
        _length = length;
        _offset = 0;
    }
    
    public void Write<T>(T value) where T : unmanaged
    {
        var size = sizeof(T);
        if (_offset + size > _length)
        {
            _length *= 2;
            var newPtr = (byte*)UnsafeUtilities.Realloc(_start, _length);
            UnsafeUtilities.Free(_start);
            _start = newPtr;
        }
        
        *(T*)_start = value;
        _offset += size;
    }
    
    public void WriteBytes(byte* bytes, int length)
    {
        if (_offset + length > _length)
        {
            _length *= 2;
            var newPtr = (byte*)UnsafeUtilities.Realloc(_start, _length);
            UnsafeUtilities.Free(_start);
            _start = newPtr;
        }
        
        UnsafeUtilities.Memcpy(_start + _offset, bytes, length);
        _offset += length;
    }
    
    public void CustomWrite<T>(T* ptr, int length) where T : unmanaged
    {
        if (_offset + length > _length)
        {
            _length *= 2;
            var newPtr = (byte*)UnsafeUtilities.Realloc(_start, _length);
            UnsafeUtilities.Free(_start);
            _start = newPtr;
        }
        
        UnsafeUtilities.Memcpy(_start + _offset, ptr, length);
        _offset += length;
    }
    
    public T Read<T>() where T : unmanaged
    {
        var size = sizeof(T);
        if (_offset + size > _length)
            return default;
        
        var value = *(T*)(_start + _offset);
        _offset += size;
        return value;
    }
    
    public byte* ReadBytes(int length)
    {
        if (_offset + length > _length)
            return null;
        
        var ptr = _start + _offset;
        _offset += length;
        return ptr;
    }
    
    public void CustomRead<T>(T* ptr, int length) where T : unmanaged
    {
        if (_offset + length > _length)
            return;
        
        UnsafeUtilities.Memcpy(ptr, _start + _offset, length);
        _offset += length;
    }
}