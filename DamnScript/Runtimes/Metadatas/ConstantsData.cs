using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Metadatas
{
    public struct ConstantsData : IDisposable
    {
        public NativeArray<NativeStringPtr> strings;
        public NativeArray<NativeStringPtr> methods;

        public ConstantsData(NativeArray<NativeStringPtr> strings, NativeArray<NativeStringPtr> methods)
        {
            this.strings = strings;
            this.methods = methods;
        }

        public void Dispose()
        {
            strings.Dispose();
            methods.Dispose();
        }
    }
}