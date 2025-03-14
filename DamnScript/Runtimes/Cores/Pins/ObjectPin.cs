using System;

namespace DamnScript.Runtimes.Cores.Pins
{
    public readonly unsafe struct ObjectPin : IEquatable<ObjectPin>
    {
        public object Target => PinHelper.GetTarget(this);
        
        public bool IsAllocated => hash != 0;
        
        public readonly long hash;
        
        public ObjectPin(long hash) => this.hash = hash;
        
        public void Free() => PinHelper.Free(this);
        
        public void* GetAddress() => PinHelper.GetAddress(this);
        
        public object FreeAndGetTarget() => PinHelper.FreeAndGetTarget(this);

        public bool Equals(ObjectPin other) => hash == other.hash;

        public override bool Equals(object obj) => obj is ObjectPin other && Equals(other);

        public override int GetHashCode() => hash.GetHashCode();
        
        public static bool operator == (ObjectPin l, ObjectPin r) => l.hash == r.hash;
        public static bool operator != (ObjectPin l, ObjectPin r) => l.hash != r.hash;
    }
}