using System;
using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.VirtualMachines.Scripts
{
    public unsafe struct ByteCodeData : IDisposable
    {
        public byte* start;
        public int length;

        public ByteCodeData(byte* start, int length)
        {
            this.start = start;
            this.length = length;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(int offset) => offset < length;

        public void Dispose() => UnsafeUtilities.Free(start);
    }
}