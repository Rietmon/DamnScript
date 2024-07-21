using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Pins;

#if UNITY_5_3_OR_NEWER
using PinHandle = System.Runtime.InteropServices.GCHandle;
#else
using PinHandle = DamnScript.Runtimes.Cores.Pins.DSObjectPin;
#endif

namespace DamnScript.Runtimes.Cores
{
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
        public static void Memcpy(void* src, void* dest, int size) => 
            Unsafe.CopyBlock(dest, src, (uint)size);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset(void* dest, byte value, int size) => 
            Unsafe.InitBlock(dest, value, (uint)size);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Memcmp<T>(T* ptr1, T* ptr2) where T : unmanaged => Memcmp(ptr1, ptr2, sizeof(T));

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
        public static void* ReferenceToPointer<T>(T value) where T : class => 
            *(void**)Unsafe.AsPointer(ref value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PointerToReference<T>(void* ptr) where T : class => Unsafe.AsRef<T>(&ptr);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PinHandle Pin<T>(T value) where T : class
        {
#if UNITY_5_3_OR_NEWER
            return PinAsDotNet(value);
#else
            return PinHelper.Pin(value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static GCHandle PinAsDotNet<T>(T value) where T : class => GCHandle.Alloc(value, GCHandleType.Pinned);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T UnpinAsDotNet<T>(GCHandle handle) where T : class
        {
            var value = (T)handle.Target;
            handle.Free();
            return value;
        }

        public static void* AddressOfPinned(PinHandle value)
        {
#if UNITY_5_3_OR_NEWER
            return value.AddrOfPinnedObject().ToPointer();
#else
            return PinHelper.GetAddress(value);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Unpin<T>(PinHandle handle) where T : class
        {
            var value = (T)handle.Target;
            handle.Free();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(ref T value) where T : unmanaged => Unsafe.AsPointer(ref value);
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int HashString(char* value, int length)
        {
            var hash = 0;
            var begin = value;
            var end = value + length;
            while (begin < end)
            {
                var c = *begin;
                if (c == '\0')
                    break;
            
                hash = (hash << 5) + hash + c;
                begin++;
            }
            return hash;
        }
    }
}