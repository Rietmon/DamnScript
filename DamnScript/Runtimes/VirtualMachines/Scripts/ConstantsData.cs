using System;
using DamnScript.Runtimes.Cores.Collections;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Runtimes.VirtualMachines.Scripts
{
    public unsafe struct ConstantsData : IDisposable
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
            for (var i = 0; i < strings.Length; i++)
                strings[i].value->Dispose();
            strings.Dispose();
            
            for (var i = 0; i < methods.Length; i++)
                methods[i].value->Dispose();
            methods.Dispose();
        }
    }
}