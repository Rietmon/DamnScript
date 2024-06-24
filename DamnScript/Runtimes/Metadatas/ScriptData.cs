namespace DamnScript.Runtimes.Metadatas;

public unsafe struct ScriptData
{
    public fixed char name[64];

    public RegionData* regions;
    
    public int regionsCount;
}