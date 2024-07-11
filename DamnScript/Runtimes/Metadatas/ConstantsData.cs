using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

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