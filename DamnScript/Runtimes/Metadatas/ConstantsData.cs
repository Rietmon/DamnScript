using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public readonly struct ConstantsData
{
    public readonly NativeArray<UnsafeStringPair> strings;
    
    public ConstantsData(NativeArray<UnsafeStringPair> strings)
    {
        this.strings = strings;
    }
}