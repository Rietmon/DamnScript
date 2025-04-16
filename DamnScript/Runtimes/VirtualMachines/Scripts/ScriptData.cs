using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Runtimes.VirtualMachines.Scripts
{
    public readonly unsafe struct ScriptDataPtr
    {
        public readonly ScriptData* value;

        public ref ScriptData RefValue => ref UnsafeUtilities.AsRef<ScriptData>(value);

        public ScriptDataPtr(ScriptData* value) => this.value = value;

        public static implicit operator ScriptDataPtr(ScriptData* value) => new(value);

        public static implicit operator ScriptData*(ScriptDataPtr ptr) => ptr.value;
    }

    public unsafe struct ScriptData
    {
        public String32 name;

        public ScriptMetadata metadata;

        public NativeArray<RegionData> regions;

        public int referencesCount;

        public RegionData* GetRegionData(String32 regionName)
        {
            var begin = regions.Begin;
            var end = regions.End;

            while (begin < end)
            {
                if (begin->name == regionName)
                    return begin;

                begin++;
            }

            return null;
        }

        public void Dispose()
        {
            if (referencesCount > 0)
                throw new Exception($"Script {name} is still referenced by {referencesCount} threads.");

            metadata.Dispose();

            for (var i = 0; i < regions.Length; i++)
                regions[i].Dispose();
            regions.Dispose();

            this = default;
        }

        public static ScriptData* Alloc()
        {
            var scriptData = UnsafeUtilities.Alloc<ScriptData>();
            scriptData->referencesCount = 0;
            return scriptData;
        }
    }
}