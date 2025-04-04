using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
	public static unsafe class PinHelper
	{
#if DAMN_SCRIPT_PINSBUCKET_SIZE_64
		public const int DefaultPinSize = 64;
#else
		public const int DefaultPinSize = 32;
#endif

		public static int PinsCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
				var count = 0;
				for (var i = 0; i < _buckets.Length; i++)
					count += (int)(DefaultPinSize - _buckets[i].freeSlots);

				return count;
			}
		}

		private static PinsBucket[] _buckets =
		{
			new(DefaultPinSize)
#if !DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
			, new(DefaultPinSize)
#endif
		};

		public static ObjectPin Pin(object obj)
		{
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "Cannot pin a null object.");
            
			var (bucketIndex, slotIndex) = FindEmptySlotOrResize();

			var objHash = obj.GetHashCode();
			var hash = objHash + slotIndex;

			var handle = new PinHandle(hash, obj);

			ref var bucket = ref _buckets[bucketIndex];
			bucket.pinnedObjects[slotIndex] = handle;
			bucket.freeSlots--;

			return new ObjectPin(hash, bucketIndex, slotIndex);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void* GetAddress(ObjectPin pin)
		{
			ref var handle = ref GetRefHandle(pin);
			return UnsafeUtilities.ReferenceToPointer(handle.target);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object GetTarget(ObjectPin pin)
		{
			ref var handle = ref GetRefHandle(pin);
			return handle.target;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(ObjectPin pin)
		{
			ref var handle = ref GetRefHandle(pin);
			handle = default;
			_buckets[pin.bucketIndex].freeSlots++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ref PinHandle GetRefHandle(ObjectPin pin)
		{
			ref var h = ref _buckets[pin.bucketIndex].pinnedObjects[pin.slotIndex];
			if (h.hash == pin.hash)
				return ref h;

			throw new Exception($"Failed to find handle with hash {pin.hash} in pinned objects array!");
		}

		private static (short bucket, short index) FindEmptySlotOrResize()
		{
			for (short i = 0; i < _buckets.Length; i++)
			{
				ref var bucket = ref _buckets[i];
				if (bucket.freeSlots > 0)
				{
					var index = bucket.FastFind();
					if (index == -1)
						throw new Exception($"Failed to find empty slot in bucket {i}! Mismatched free slots count!");
					
					return (i, index);
				}
			}

			var oldLength = _buckets.Length;
			var newSize = oldLength * 2;
			if (newSize > short.MaxValue)
				throw new Exception($"Failed to resize buckets! New size {newSize} is greater than max value {short.MaxValue}!");
			
			Array.Resize(ref _buckets, newSize);
			for (var i = oldLength; i < newSize; i++)
				_buckets[i] = new PinsBucket(DefaultPinSize);

			return ((short)oldLength, 0);
		}
	}
}