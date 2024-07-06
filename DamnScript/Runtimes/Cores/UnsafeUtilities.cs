using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

public static unsafe class UnsafeUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* Alloc(int size) => Marshal.AllocHGlobal(size).ToPointer();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* Alloc<T>() where T : unmanaged => (T*)Alloc(sizeof(T));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* ReAlloc(void* ptr, int size) => Marshal.ReAllocHGlobal(new IntPtr(ptr), new IntPtr(size)).ToPointer();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Free(void* ptr) => Marshal.FreeHGlobal(new IntPtr(ptr));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Memcpy(void* dest, void* src, int size) => Unsafe.CopyBlock(dest, src, (uint)size);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Memset(void* dest, byte value, int size) => Unsafe.InitBlock(dest, value, (uint)size);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Memcmp<T>(T* ptr1, T* ptr2) where T : unmanaged => Unsafe.AreSame(ref *ptr1, ref *ptr2);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Memcmp(void* ptr1, void* ptr2, int size)
    {
        var ptr1Byte = (byte*)ptr1;
        var ptr2Byte = (byte*)ptr2;
        for (var i = 0; i < size; i++)
        {
            if (ptr1Byte[i] != ptr2Byte[i])
                return false;
        }
        return true;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Strlen(char* str)
    {
        var length = 0;
        while (*str++ != '\0')
            length++;
        return length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void* ReferenceToPointer<T>(T value) where T : class => *(void**)Unsafe.AsPointer(ref value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T PointerToReference<T>(void* ptr) where T : class => Unsafe.AsRef<T>(&ptr);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HashString(char* value, int length)
    {
        var hash = 0;
        var begin = value;
        var end = value + length;
        while (begin < end)
        {
            hash = (hash << 5) + hash + *begin;
            begin++;
        }
        return hash;
    }
}