using System;

namespace DamnScript.Runtimes.Cores.Pins
{
    public readonly unsafe struct ObjectPin : IEquatable<ObjectPin>
    {
        public object Target => PinHelper.GetTarget(this);
        
        public bool IsAllocated => hash != 0;
        
        public readonly long hash;
        
        public ObjectPin(long hash) => this.hash = hash;
        
        /// <summary>
        /// Let GC collect the object.
        /// </summary>
        public void Free() => PinHelper.Free(this);
        
        /// <summary>
        /// Return TEMPORARY address of the object.
        /// !!! DO NOT CACHE THID VALUE IN .NET CORE!!! .NET CORE GC CAN MOVE OBJECTS IN MEMORY!!!
        /// </summary>
        /// <returns></returns>
        public void* GetAddress() => PinHelper.GetAddress(this);
        
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