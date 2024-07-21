using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Types
{
    /// <summary>
    /// String buffer with fixed length of 32, without last null terminator.
    /// </summary>
    public unsafe struct String32
    {
        public const int Length = 32;

        public ref char this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref data[index];
        }
    
        public fixed char data[Length];
    
        public String32(char* source, int length)
        {
            if (length > 32)
                throw new ArgumentException("String length must be less than or equal to 32.");
        
            fixed (char* dest = data)
                UnsafeUtilities.Memcpy(source, dest, length * sizeof(char));
        }
    
        public String32(string value)
        {
            var length = value.Length;
            if (length > 32)
                throw new ArgumentException("String length must be less than or equal to 32.");
        
            fixed (char* dest = data)
            {
                fixed (char* src = value)
                {
                    UnsafeUtilities.Memcpy(src, dest, length * sizeof(char));
                }
            }
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
}