using System;
using System.Runtime.CompilerServices;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeString* GetNativeString(int index) => 
            constants.strings[index].value;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeString* GetMethodName(int index) => 
            constants.methods[index].value;

        public void Dispose()
        {
            constants.Dispose();
        }
    }
}