﻿using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas
{
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