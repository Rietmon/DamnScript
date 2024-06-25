using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas;

public readonly unsafe struct RegionData
{
    public readonly String32 name;
    
    public readonly ByteCodeData byteCode;
    
    public RegionData(string name, ByteCodeData byteCode)
    {
        fixed (char* namePtr = name)
        {
            for (var i = 0; i < 64; i++)
            {
                this.name[i] = namePtr[i];
            }
        }
        
        this.byteCode = byteCode;
    }
}