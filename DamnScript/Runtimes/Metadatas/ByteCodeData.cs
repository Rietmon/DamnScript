using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct ByteCodeData
{
    public readonly byte* start;
    public readonly byte* end;
    public readonly int length;
    
    public ByteCodeData(byte* start, byte* end)
    {
        this.start = start;
        this.end = end;
        length = (int)(end - start);
    }
    
    public ByteCodeData(byte* start, int length)
    {
        this.start = start;
        end = start + length;
        this.length = length;
    }
    
    [MethodImpl(MethodImplOptions.NoOptimization)]
    public bool IsInRange(int offset) => offset < length;
}