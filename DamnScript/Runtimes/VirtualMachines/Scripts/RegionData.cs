using System;
using System.Diagnostics;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Strings;
using DamnScript.Runtimes.Debugs;

namespace DamnScript.Runtimes.VirtualMachines.Scripts
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
    
    public struct RegionData : IDisposable
    {
        public String32 name;

        public ByteCodeData byteCode;
        
#if DEBUG
        public string DebugOnlyDisassemble => ScriptDisassembler.DisassembleRegionToString(new RegionDataPtr(ref this));
#endif

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