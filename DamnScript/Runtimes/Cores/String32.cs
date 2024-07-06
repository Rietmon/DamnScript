using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public unsafe struct String32
{
    public const int Length = 32;

    public ref char this[int index] => ref data[index];
    
    public fixed char data[Length + 1];
    
    public String32(char* value, int length)
    {
        if (length > 32)
            throw new ArgumentException("String length must be less than or equal to 32.");
        
        fixed (char* ptr = data)
            UnsafeUtilities.Memcpy(ptr, value, length * sizeof(char));
        
        data[length] = '\0';
    }
    
    public String32(string value)
    {
        var length = value.Length;
        if (length > 32)
            throw new ArgumentException("String length must be less than or equal to 32.");
        
        fixed (char* strPtr = data)
        {
            fixed (char* valuePtr = value)
            {
                UnsafeUtilities.Memcpy(valuePtr, strPtr, length * sizeof(char));
            }
        }
        
        data[length] = '\0';
    }

    public override string ToString()
    {
        fixed (char* ptr = data)
            return new string(ptr);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator==(String32 left, String32 right)
    {
        var begin = left.data;
        var end = right.data;
        for (var i = 0; i < Length; i++)
        {
            if (*begin != *end)
                return false;
            
            begin++;
            end++;
        }
        return true;
    }

    public static bool operator !=(String32 left, String32 right) => !(left == right);
    
    
    public bool Equals(String32 other) => this == other;

    public override bool Equals(object obj) => obj is String32 other && Equals(other);

    public override int GetHashCode()
    {
        fixed (char* ptr = data)
            return UnsafeUtilities.HashString(ptr, Length);
    }
}