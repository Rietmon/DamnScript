using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Allocators;

namespace DamnScript.Runtimes.Memories.Allocators;

public unsafe struct Allocator
{
    public int AllocationSize => sizeof(Allocator) + bucketsCount * sizeof(Bucket) + zonesCount * sizeof(Zone);
    
    public int size;
    public int bucketsCount;
    public int zonesCount;
    
    public Bucket* buckets;

    public static Allocator* Create(int size, int bucketsCount, int zonesCount)
    {
        var allocatorSize = sizeof(Allocator) - sizeof(Bucket*);
        var zonesPerBucketSize = zonesCount * sizeof(Zone);
        var bucketsSize = sizeof(Bucket) - sizeof(Zone*) + zonesPerBucketSize;
        var allocationSize = allocatorSize + zonesPerBucketSize + bucketsSize;

        var allocation = Marshal.AllocHGlobal(allocationSize).ToPointer();
        Unsafe.InitBlock(allocation, 0, (uint)allocationSize);
        
        var allocator = (Allocator*)allocation;
    }

    public void Initialize(int size, int bucketsCount, int zonesCount)
    {
        this.size = size;
        this.bucketsCount = bucketsCount;
        this.zonesCount = zonesCount;
        
        for (var i = 0; i < bucketsCount; i++)
        {
            var bucket = bucke
            bucket->size = size / bucketsCount;
            bucket->zones = (Zone*)((byte*)buckets + sizeof(Allocator) + i * sizeof(Bucket));
        }
    }
}