using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Runtimes.VirtualMachines.Scripts
{
    public unsafe struct ScriptMetadata : IDisposable
    {
        public ConstantsData constants;

        public ScriptMetadata(ConstantsData constants)
        {
            this.constants = constants;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeString* GetConstString(int index) => 
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