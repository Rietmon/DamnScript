using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Metadatas
{
    public readonly unsafe struct ScriptDataPtr
    {
        public readonly ScriptData* value;

        public ref ScriptData RefValue => ref *value;

        public ScriptDataPtr(ScriptData* value) => this.value = value;

        public static implicit operator ScriptDataPtr(ScriptData* value) => new(value);

        public static implicit operator ScriptData*(ScriptDataPtr ptr) => ptr.value;
    }

    public unsafe struct ScriptData
    {
        public String32 name;

        public ScriptMetadata metadata;

        public NativeArray<RegionData> regions;

        public RegionData* GetRegionData(String32 regionName) => GetRegionData(regionName.GetHashCode());

        public RegionData* GetRegionData(int hash)
        {
            var begin = regions.Begin;
            var end = regions.End;

            while (begin < end)
            {
                if (begin->name.GetHashCode() == hash)
                    return begin;

                begin++;
            }

            return null;
        }

        internal void Dispose()
        {
            metadata.Dispose();
            regions.Dispose();
        }
    }
}