using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
    [DebuggerDisplay("{Target}")]
    public readonly unsafe struct PinHandle : IEquatable<PinHandle>
    {
        /// <summary>
        /// Target reference of the object.
        /// </summary>
        public object Target
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PinHelper.GetTarget(this);
        }

        /// <summary>
        /// Return TEMPORARY address of the object.
        /// !!! DO NOT CACHE THIS VALUE IN .NET CORE!!! .NET CORE GC CAN MOVE OBJECTS IN MEMORY!!!
        /// </summary>
        /// <returns>Address of pinned reference</returns>
        public void* Address
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => PinHelper.GetAddress(this);
        }
        
        /// <summary>
        /// Hash of the object.
        /// </summary>
        public readonly int hash;
        
        public readonly short bucketIndex;
        public readonly short slotIndex;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PinHandle(int hash, short bucketIndex, short slotIndex)
        {
            this.hash = hash;
            this.bucketIndex = bucketIndex;
            this.slotIndex = slotIndex;
        }
        
        /// <summary>
        /// Let GC collect the object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => PinHelper.Free(this);

        public bool Equals(PinHandle other) => hash == other.hash;

        public override bool Equals(object obj) => obj is PinHandle other && Equals(other);

        public override int GetHashCode() => hash.GetHashCode();
        
        public static bool operator == (PinHandle l, PinHandle r) => l.hash == r.hash;
        public static bool operator != (PinHandle l, PinHandle r) => l.hash != r.hash;
    }
}