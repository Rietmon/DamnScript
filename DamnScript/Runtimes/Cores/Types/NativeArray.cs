﻿using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Types
{
    public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
    {
        public int Length { get; }
    
        public T* Begin { get; }
    
        public T* End => Begin + Length;

        public bool IsValid => Begin != null;

        public ref T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)] get => ref Begin[index];
        }
    
        public NativeArray(int length) : this()
        {
            Begin = (T*)UnsafeUtilities.Alloc(sizeof(T) * length);
        }
    
        public NativeArray(int length, T* begin)
        {
            Length = length;
            Begin = begin;
        }
    
        public NativeList<T> ToListAlloc()
        {
            var list = new NativeList<T>(Length);
            list.AddRange(Begin, Length);
            return list;
        }
    
        public void Dispose()
        {
            UnsafeUtilities.Free(Begin);
            this = default;
        }
    }
}