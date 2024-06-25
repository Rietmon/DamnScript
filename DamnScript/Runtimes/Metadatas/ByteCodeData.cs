using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct ByteCodeData
{
    public readonly byte* start;
    public readonly int length;
    public readonly byte* end;
    
    public ByteCodeData(byte* start, byte* end)
    {
        this.start = start;
        length = (int)(end - start);
        this.end = end;
    }
    
    public ByteCodeData(byte* start, int length)
    {
        this.start = start;
        this.length = length;
        end = start + length;
    }
    
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public bool IsInRange(int offset) => offset < length;
}