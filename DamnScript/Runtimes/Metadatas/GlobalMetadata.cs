using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public static unsafe class GlobalMetadata
{
    public static ConstantsData constantsData;

    public static int AddOrGetIndex(UnsafeString* str)
    {
        if (constantsData.strings.Count == 0)
        {
            constantsData.strings.Add(new UnsafeStringPtr(str));
            return 0;
        }
    }
    
    public static int GetIndex(UnsafeString* str)
    {
        for (var i = 0; i < constantsData.strings.Count; i++)
        {
            if (constantsData.strings[i].Equals(str))
            {
                return i;
            }
        }
        
        return -1;
    }
}