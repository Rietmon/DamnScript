using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct ScriptDataPtr
{
    public readonly ScriptData* value; 
    
    public ref ScriptData RefValue => ref *value;
    
    public ScriptDataPtr(ScriptData* value) => this.value = value;

    public static implicit operator ScriptDataPtr(ScriptData* value) => new(value);
    
    public static implicit operator ScriptData*(ScriptDataPtr ptr) => ptr.value;
}

public unsafe struct ScriptData : IDisposable
{
    public String32 name;

    public ScriptMetadata metadata;
    
    public NativeArray<RegionData> regions;

    public RegionData* GetRegionData(String32 regionName)
    {
        var begin = regions.Begin;
        var end = regions.End;
        
        while (begin < end)
        {
            if (UnsafeUtilities.Memcmp(&regionName, &begin->name))
                return begin;
            
            begin++;
        }
        
        return null;
    }

    void IDisposable.Dispose()
    {
        metadata.Dispose();
        regions.Dispose();
    }

    internal void Internal_Dispose()
    {
        metadata.Dispose();
        regions.Dispose();
    }
}