using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct String32
{
    public fixed char value[32];
    
    public char this[int index]
    {
        get => value[index];
        set => this.value[index] = value;
    }
    
    public static String32 FromString(string value)
    {
        var str = new String32();
        fixed (char* ptr = value)
        {
            UnsafeUtilities.Memcpy(str.value, ptr, value.Length * sizeof(char));
            str[value.Length] = '\0';
        }
        return str;
    }

    public override string ToString()
    {
        fixed (char* ptr = value)
            return new string(ptr);
    }
}