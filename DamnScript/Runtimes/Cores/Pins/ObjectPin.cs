using System;
using System.Diagnostics;

namespace DamnScript.Runtimes.Cores.Pins
{
    [DebuggerDisplay("{Target}")]
    public readonly unsafe struct ObjectPin : IEquatable<ObjectPin>
    {
        /// <summary>
        /// Target reference of the object.
        /// </summary>
        public object Target => PinHelper.GetTarget(this);
        
        /// <summary>
        /// Return TEMPORARY address of the object.
        /// !!! DO NOT CACHE THID VALUE IN .NET CORE!!! .NET CORE GC CAN MOVE OBJECTS IN MEMORY!!!
        /// </summary>
        /// <returns>Address of pinned reference</returns>
        public void* Address => PinHelper.GetAddress(this);
        
        /// <summary>
        /// Hash of the object.
        /// </summary>
        public readonly long hash;
        
        public ObjectPin(long hash) => this.hash = hash;
        
        /// <summary>
        /// Let GC collect the object.
        /// </summary>
        public void Free() => PinHelper.Free(this);
        
        /// <summary>
        /// Return pinned object and free it.
        /// </summary>
        /// <returns></returns>
        public object FreeAndGetTarget() => PinHelper.FreeAndGetTarget(this);

        public bool Equals(ObjectPin other) => hash == other.hash;

        public override bool Equals(object obj) => obj is ObjectPin other && Equals(other);

        public override int GetHashCode() => hash.GetHashCode();
        
        public static bool operator == (ObjectPin l, ObjectPin r) => l.hash == r.hash;
        public static bool operator != (ObjectPin l, ObjectPin r) => l.hash != r.hash;
    }
}