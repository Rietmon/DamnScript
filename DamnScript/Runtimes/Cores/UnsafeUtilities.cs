using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Debugs;

#if UNITY_5_3_OR_NEWER
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace DamnScript.Runtimes.Cores
{
    public static unsafe class UnsafeUtilities
    {
	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	    public static void* Alloc(int size)
	    {
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
		    Debugging.Log($"[{nameof(UnsafeUtilities)}] ({nameof(Alloc)}) Allocating {size.ToString()} bytes.");
#endif
		    var ptr = Marshal.AllocHGlobal(size).ToPointer();
		    
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
		    Debugging.Log($"[{nameof(UnsafeUtilities)}] ({nameof(Alloc)}) Allocated at {new IntPtr(ptr):X}.");
#endif
		    if (ptr != null)
			    return ptr;
		    
		    throw new OutOfMemoryException($"Failed to allocate {size.ToString()} bytes!");
	    }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* Alloc<T>() where T : unmanaged => (T*)Alloc(sizeof(T));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* ReAlloc(void* ptr, int size)
        {
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
	        Debugging.Log($"[{nameof(UnsafeUtilities)}] ({nameof(ReAlloc)}) Reallocating at {new IntPtr(ptr):X} to {size.ToString()} bytes.");
#endif

	        var newPtr = Marshal.ReAllocHGlobal(new IntPtr(ptr), new IntPtr(size)).ToPointer();
	        
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
	        Debugging.Log($"[{nameof(UnsafeUtilities)}] ({nameof(ReAlloc)}) Reallocated at {new IntPtr(newPtr):X}.");
#endif
	        if (newPtr != null)
		        return newPtr;
		    
	        throw new OutOfMemoryException($"Failed to reallocate {size.ToString()} bytes!");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* ptr)
        {
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
	        Debugging.Log($"[{nameof(UnsafeUtilities)}] ({nameof(Free)}) Freeing at {new IntPtr(ptr):X}.");
#endif
	        
	        Marshal.FreeHGlobal(new IntPtr(ptr));
	        
#if DAMN_SCRIPT_ENABLE_MEMORY_DEBUG
	        Debugging.Log($"[{nameof(UnsafeUtilities)}] ({nameof(Free)}) Freed.");
#endif
        }
    
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memcpy(void* src, void* dest, int size) => 
#if UNITY_5_3_OR_NEWER
            UnsafeUtility.MemCpy(dest, src, size);
#else
            Unsafe.CopyBlock(dest, src, (uint)size);
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset(void* dest, byte value, int size) =>
#if UNITY_5_3_OR_NEWER
            UnsafeUtility.MemSet(dest, value, size);
#else
            Unsafe.InitBlock(dest, value, (uint)size);
#endif
    
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

#if UNITY_5_3_OR_NEWER
        [StructLayout(LayoutKind.Sequential)]
        private struct PointerToReferenceCastHelper
        {
            public object value;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* ReferenceToPointer<T>(T value) where T : class
        {
#if UNITY_5_3_OR_NEWER
            var castHelper = new PointerToReferenceCastHelper() { value = value };
            var ptr = (void**)UnsafeUtility.AddressOf(ref castHelper);
            return *ptr;
#else
            return *(void**)Unsafe.AsPointer(ref value);
#endif
        }

#if UNITY_5_3_OR_NEWER
        [StructLayout(LayoutKind.Sequential)]
        private struct ReferenceToPointerCastHelper<T>
        {
            public T value;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T PointerToReference<T>(void* ptr) where T : class
        {
#if UNITY_5_3_OR_NEWER
            
            var castHelper = UnsafeUtility.AsRef<ReferenceToPointerCastHelper<T>>(&ptr);
            return castHelper.value;
#else
            return Unsafe.AsRef<T>(&ptr);
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DSObjectPin Pin<T>(T value) where T : class
        {
            return PinHelper.Pin(value);
        }

        public static void* AddressOfPinned(DSObjectPin value)
        {
            return PinHelper.GetAddress(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Unpin<T>(DSObjectPin handle) where T : class
        {
            var value = (T)handle.Target;
            handle.Free();
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AsPointer<T>(ref T value) where T : unmanaged =>
#if UNITY_5_3_OR_NEWER
            UnsafeUtility.AddressOf(ref value);
#else
            Unsafe.AsPointer(ref value);
#endif
    
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