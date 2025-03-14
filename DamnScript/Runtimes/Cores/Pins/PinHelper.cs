using System;
using System.Collections.Generic;

namespace DamnScript.Runtimes.Cores.Pins
{
    // Rietmon: Rewrite to a real hash set system
    public static unsafe class PinHelper
    {
        private static readonly HashSet<PinHandle> pinnedObjects = new(32);
        
        public static ObjectPin Pin(object obj)
        {
            var hash = obj.GetHashCode() + pinnedObjects.Count;
            pinnedObjects.Add(new PinHandle(hash, obj));
            return new ObjectPin(hash);
        }
        
        public static void* GetAddress(ObjectPin pin)
        {
            var handle = FindHandle(pin.hash);
            if (handle == default)
                return null;
            
            return UnsafeUtilities.ReferenceToPointer(handle.target);
        }
        
        public static object GetTarget(ObjectPin pin)
        {
            var handle = FindHandle(pin.hash);
            if (handle == default)
                return null;
            
            return handle.target;
        }
        
        public static void Free(ObjectPin pin)
        {
            var handle = FindHandle(pin.hash);
            if (handle == default)
                return;
            
            pinnedObjects.Remove(handle);
        }
        
        public static object FreeAndGetTarget(ObjectPin pin)
        {
            var handle = FindHandle(pin.hash);
            if (handle == default)
                return null;
            
            pinnedObjects.Remove(handle);
            return handle.target;
        }
        
        private static PinHandle FindHandle(long hash)
        {
            foreach (var obj in pinnedObjects)
            {
                if (obj.hash == hash)
                    return obj;
            }
            return default;
        }

        private readonly struct PinHandle : IEquatable<PinHandle>
        {
            public readonly long hash;
            public readonly object target;
            
            public PinHandle(long hash, object target)
            {
                this.hash = hash;
                this.target = target;
            }
            
            public static bool operator ==(PinHandle l, PinHandle r) => r.hash == l.hash && l.target == r.target;
            public static bool operator !=(PinHandle l, PinHandle r) => !(r == l);

            public bool Equals(PinHandle other) => hash == other.hash && Equals(target, other.target);

            public override bool Equals(object obj) => obj is PinHandle other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(hash, target);
        }
    }
}