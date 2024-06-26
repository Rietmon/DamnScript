using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public unsafe struct ScriptData
{
    public fixed char name[64];

    public NativeArray<RegionData> regions;
}