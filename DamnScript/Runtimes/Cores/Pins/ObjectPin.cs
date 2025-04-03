using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
    [DebuggerDisplay("{Target}")]
    public readonly unsafe struct ObjectPin : IEquatable<ObjectPin>
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
        /// Index of the object in the pinned list.
        /// </summary>
        public readonly int index;
        
        /// <summary>
        /// Hash of the object.
        /// </summary>
        public readonly int hash;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ObjectPin(int index, int hash)
        {
            this.index = index;
            this.hash = hash;
        }

        /// <summary>
        /// Let GC collect the object.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => PinHelper.Free(this);

        public bool Equals(ObjectPin other) => hash == other.hash;

        public override bool Equals(object obj) => obj is ObjectPin other && Equals(other);

        public override int GetHashCode() => hash.GetHashCode();
        
        public static bool operator == (ObjectPin l, ObjectPin r) => l.hash == r.hash;
        public static bool operator != (ObjectPin l, ObjectPin r) => l.hash != r.hash;
    }
}