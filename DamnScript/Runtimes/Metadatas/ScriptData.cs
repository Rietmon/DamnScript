﻿using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct ScriptDataPtr
{
    public readonly ScriptData* value; 
    
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
            if (UnsafeUtilities.Memcmp(&regionName, &begin->name, sizeof(String32)))
                return begin;
            
            begin++;
        }
        
        return null;
    }

    public void Dispose()
    {
        metadata.Dispose();
        regions.Dispose();
    }
}