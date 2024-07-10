using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Arrays;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Runtimes.Metadatas
{
    public struct ConstantsData : IDisposable
    {
        public NativeArray<UnsafeStringPair> strings;
    
        public ConstantsData(NativeArray<UnsafeStringPair> strings)
    {
        this.strings = strings;
    }

        public void Dispose()
    {
        strings.Dispose();
    }
    }
}