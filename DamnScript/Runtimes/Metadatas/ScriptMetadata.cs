using System;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Strings;

namespace DamnScript.Runtimes.Metadatas
{
    public unsafe struct ScriptMetadata : IDisposable
    {
        private ConstantsData _constants;
    
        public ScriptMetadata(ConstantsData constants)
    {
        _constants = constants;
    }

        public UnsafeString* GetUnsafeString(int hash)
    {
        var begin = _constants.strings.Begin;
        var end = _constants.strings.End;
        
        while (begin < end)
        {
            if (begin->hash == hash)
                return begin->value;
            
            begin++;
        }
        
        return null;
    }

        public void Dispose()
    {
        _constants.Dispose();
    }
    }
}