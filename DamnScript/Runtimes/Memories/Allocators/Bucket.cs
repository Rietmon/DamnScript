using DamnScript.Runtimes.Allocators;

namespace DamnScript.Runtimes.Memories.Allocators;

public unsafe struct Bucket
{
    public int size;
    
    public int zonesCount;
    
    // Rietmon: Tail
    public Zone* zones;

    public Bucket(int size)
    {
        this.size = size;
    }
}