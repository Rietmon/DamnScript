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
}