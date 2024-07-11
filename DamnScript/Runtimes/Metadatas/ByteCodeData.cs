using System.Runtime.CompilerServices;
using DamnScript.Runtimes.Cores;

namespace DamnScript.Runtimes.Metadatas
{
    public readonly unsafe struct ByteCodeData
    {
        public readonly byte* start;
        public readonly int length;

        public ByteCodeData(byte* start, int length)
        {
            this.start = start;
            this.length = length;
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.AggressiveInlining)]
        public bool IsInRange(int offset) => offset < length;
    }
}