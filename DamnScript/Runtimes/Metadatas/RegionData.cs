using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Metadatas
{
    public readonly unsafe struct RegionDataPtr
    {
        public readonly RegionData* value;

        public ref RegionData RefValue => ref UnsafeUtilities.AsRef<RegionData>(value);

        public RegionDataPtr(RegionData* value) => this.value = value;

        public RegionDataPtr(ref RegionData value) => this.value = UnsafeUtilities.AsPointer(ref value);

        public static implicit operator RegionDataPtr(RegionData* value) => new(value);
        public static implicit operator RegionData*(RegionDataPtr ptr) => ptr.value;
    }
    
    public readonly struct RegionData : IDisposable
    {
        public readonly String32 name;

        public readonly ByteCodeData byteCode;

        public RegionData(String32 name, ByteCodeData byteCode)
        {
            this.name = name;
            this.byteCode = byteCode;
        }

        public void Dispose()
        {
            byteCode.Dispose();
        }
    }
}