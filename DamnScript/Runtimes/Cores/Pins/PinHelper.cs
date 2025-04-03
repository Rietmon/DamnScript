using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
    public static unsafe class PinHelper
    {
        public static int PinsCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var count = 0;
                for (var i = 0; i < _pinnedObjects.Length; i++)
                {
                    if (_pinnedObjects[i].hash != 0)
                        count++;
                }
                return count;
            }
        }
        
        private static PinHandle[] _pinnedObjects = new PinHandle[32];
        
        public static ObjectPin Pin(object obj)
        {
#if DAMN_SCRIPT_ENABLE_ADDITIONAL_CHECKS
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Cannot pin a null object.");
#endif
            var slot = FindEmptySlot();
            var objHash = obj.GetHashCode();
            var hash = objHash + slot;
            var handle = new PinHandle(hash, obj);
            _pinnedObjects[slot] = handle;
            return new ObjectPin(slot, hash);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* GetAddress(ObjectPin pin)
        {
            ref var handle = ref FindHandle(pin);
            return UnsafeUtilities.ReferenceToPointer(handle.target);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object GetTarget(ObjectPin pin)
        {
            ref var handle = ref FindHandle(pin);
            return handle.target;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(ObjectPin pin)
        {
            ref var handle = ref FindHandle(pin);
            handle = default;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ref PinHandle FindHandle(ObjectPin pin)
        {
            ref var handle = ref _pinnedObjects[pin.index];
            if (handle.hash == pin.hash)
                return ref handle;
            
            throw new Exception($"Failed to find handle with hash {pin.hash} in pinned objects array!");
        }
        
        private static int FindEmptySlot()
        {
            for (var i = 0; i < _pinnedObjects.Length; i++)
            {
                if (_pinnedObjects[i].hash == 0)
                    return i;
            }
            
            var oldLength = _pinnedObjects.Length;
            var newSize = oldLength * 2;
            Array.Resize(ref _pinnedObjects, newSize);
            for (var i = oldLength; i < newSize; i++)
            {
                if (_pinnedObjects[i].hash == 0)
                    return i;
            }
           
            throw new Exception($"Failed to find empty slot in pinned objects array even after realloc!");
        }

        private readonly struct PinHandle
        {
            public readonly object target;
#if DAMN_SCRIPT_PINNING_DEBUG
            public readonly string stack;
#endif
            public readonly int hash;
            
            public PinHandle(int hash, object target)
            {
                this.hash = hash;
#if DAMN_SCRIPT_PINNING_DEBUG
                this.stack = Environment.StackTrace;
#endif
                this.target = target;
            }
        }
    }
}