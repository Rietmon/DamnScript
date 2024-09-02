using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Metadatas
{
    public struct ConstantsData : IDisposable
    {
        public NativeArray<UnsafeStringPtr> strings;
        public NativeArray<UnsafeStringPtr> methods;

        public ConstantsData(NativeArray<UnsafeStringPtr> strings, NativeArray<UnsafeStringPtr> methods)
        {
            this.strings = strings;
            this.methods = methods;
        }

        public void Dispose()
        {
            strings.Dispose();
        }
    }
}