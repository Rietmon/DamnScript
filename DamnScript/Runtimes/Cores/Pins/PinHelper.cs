using System.Collections.Generic;

namespace DamnScript.Runtimes.Cores.Pins
{
    public static unsafe class PinHelper
    {
        private static readonly HashSet<(long hash, object target)> pinnedObjects = new(32);
        
        public static ObjectPin Pin(object obj)
        {
            var hash = obj.GetHashCode() + pinnedObjects.Count;
            pinnedObjects.Add((hash, obj));
            return new ObjectPin(hash);
        }
        
        public static void* GetAddress(ObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                if (pin.hash == obj.hash)
                    return UnsafeUtilities.ReferenceToPointer(obj.target);
            }
            return null;
        }
        
        public static object GetTarget(ObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                if (pin.hash == obj.hash)
                    return obj.target;
            }
            return null;
        }
        
        public static void Free(ObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                if (pin.hash != obj.hash) 
                    continue;
                
                pinnedObjects.Remove(obj);
                return;
            }
        }
        
        public static object FreeAndGetTarget(ObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                var hash = obj.GetHashCode();
                if (hash != pin.hash) 
                    continue;
                
                pinnedObjects.Remove(obj);
                return obj.target;
            }

            return null;
        }
    }
}