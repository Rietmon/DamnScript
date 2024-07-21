using System.Collections.Generic;

namespace DamnScript.Runtimes.Cores.Pins
{
    // Rietmon: Thanks to .NET we can't pin the objects which are contains other references.
    // Only one way I found its storage them in the dictionary and pin them by the key.
    public static unsafe class PinHelper
    {
        private static readonly HashSet<(long hash, object target)> pinnedObjects = new(32);
        
        public static DSObjectPin Pin(object obj)
        {
            var hash = obj.GetHashCode() + pinnedObjects.Count;
            pinnedObjects.Add((hash, obj));
            return new DSObjectPin(hash);
        }
        
        public static void* GetAddress(DSObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                if (pin.hash == obj.hash)
                    return UnsafeUtilities.ReferenceToPointer(obj.target);
            }
            return null;
        }
        
        public static object GetTarget(DSObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                if (pin.hash == obj.hash)
                    return obj.target;
            }
            return null;
        }
        
        public static void Free(DSObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                if (pin.hash != obj.hash) 
                    continue;
                
                pinnedObjects.Remove(obj);
                return;
            }
        }
        
        public static object FreeAndGetTarget(DSObjectPin pin)
        {
            foreach (var obj in pinnedObjects)
            {
                var hash = obj.GetHashCode();
                if (hash != pin.hash) 
                    continue;
                
                pinnedObjects.Remove(obj);
                return obj;
            }

            return null;
        }
    }
}