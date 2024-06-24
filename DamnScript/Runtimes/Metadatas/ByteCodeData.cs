namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct ByteCodeData
{
    public readonly byte* start;
    public readonly byte* end;
    
    public ByteCodeData(byte* start, byte* end)
    {
        this.start = start;
        this.end = end;
    }
    
    public ByteCodeData(byte* start, int length)
    {
        this.start = start;
        end = start + length;
    }
}