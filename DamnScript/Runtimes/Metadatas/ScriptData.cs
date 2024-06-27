using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public unsafe struct ScriptData
{
    public String32 name;

    public ScriptMetadata metadata;
    
    public NativeArray<RegionData> regions;
}