using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public struct ConstantsData
{
    public NativeList<UnsafeStringPtr> strings = new(16);
    
    public ConstantsData() { }
}