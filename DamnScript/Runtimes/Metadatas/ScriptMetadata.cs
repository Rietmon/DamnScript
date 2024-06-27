using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct ScriptMetadata
{
    private readonly ConstantsData _constants;
    
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
}