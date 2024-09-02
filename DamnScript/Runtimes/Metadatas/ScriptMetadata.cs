using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Types;

namespace DamnScript.Runtimes.Metadatas
{
    public unsafe struct ScriptMetadata : IDisposable
    {
        public ConstantsData constants;

        public ScriptMetadata(ConstantsData constants)
        {
            this.constants = constants;
        }

        public UnsafeString* GetUnsafeString(int index) => 
            constants.strings[index].value;
        
        public UnsafeString* GetMethodName(int index) => 
            constants.methods[index].value;

        public void Dispose()
        {
            constants.Dispose();
        }
    }
}