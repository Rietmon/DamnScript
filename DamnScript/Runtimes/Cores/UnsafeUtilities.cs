using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DamnScript.Runtimes.Cores;

internal static unsafe class UnsafeUtilities
{
    public static void* Alloc(int size) => Marshal.AllocHGlobal(size).ToPointer();
    
    public static void* Realloc(void* ptr, int size) => Marshal.ReAllocHGlobal(new IntPtr(ptr), new IntPtr(size)).ToPointer();
    
    public static void Free(void* ptr) => Marshal.FreeHGlobal(new IntPtr(ptr));
    
    public static void Memcpy(void* dest, void* src, int size) => Unsafe.CopyBlock(dest, src, (uint)size);
    
    public static void Memset(void* dest, byte value, int size) => Unsafe.InitBlock(dest, value, (uint)size);
    
    public static bool Memcmp<T>(T* ptr1, T* ptr2) where T : unmanaged => Unsafe.AreSame(ref *ptr1, ref *ptr2);

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
    
    public static void Strcpy(char* dest, char* src)
    {
        var i = 0;
        while (src[i] != '\0')
        {
            dest[i] = src[i];
            i++;
        }
        dest[i] = '\0';
    }

    public static void* ReferenceToPointer<T>(T value) where T : class => *(void**)Unsafe.AsPointer(ref value);

    public static T PointerToReference<T>(void* ptr) where T : class => Unsafe.AsRef<T>(&ptr);
    
    public static T* FixedBufferToPtr<T>(ref T value) where T : unmanaged => (T*)Unsafe.AsPointer(ref value);
}