using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Metadatas
{
    public readonly unsafe struct RegionDataPtr
    {
        public readonly RegionData* value;

        public ref RegionData RefValue => ref *value;

        public RegionDataPtr(RegionData* value) => this.value = value;

        public static implicit operator RegionDataPtr(RegionData* value) => new(value);

        public static implicit operator RegionData*(RegionDataPtr ptr) => ptr.value;
    }
    
    public readonly struct RegionData
    {
        public readonly String32 name;

        public readonly ByteCodeData byteCode;

        public RegionData(String32 name, ByteCodeData byteCode)
        {
            this.name = name;
            this.byteCode = byteCode;
        }
    }
}