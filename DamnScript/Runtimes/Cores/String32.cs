using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct String32
{
    public fixed char value[32];
    
    public String32(char* value, int length)
    {
        if (length > 32)
            throw new ArgumentException("String length must be less than or equal to 32.");
        
        fixed (char* ptr = this.value)
            UnsafeUtilities.Memcpy(ptr, value, length * sizeof(char));
    }
    
    public String32(string value)
    {
        if (value.Length > 32)
            throw new ArgumentException("String length must be less than or equal to 32.");
        
        fixed (char* strPtr = this.value)
        {
            fixed (char* valuePtr = value)
            {
                UnsafeUtilities.Memcpy(valuePtr, strPtr, value.Length * sizeof(char));
            }
        }
    }

    public override string ToString()
    {
        fixed (char* ptr = value)
            return new string(ptr);
    }
}