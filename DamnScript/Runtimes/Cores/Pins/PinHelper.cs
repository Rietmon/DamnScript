using System;
using System.Runtime.CompilerServices;

namespace DamnScript.Runtimes.Cores.Pins
{
	public static unsafe class PinHelper
	{
#if DAMN_SCRIPT_PINSBUCKET_SIZE_64
		public const int DefaultPinsBucketSize = 64;
#else
		public const int DefaultPinsBucketSize = 32;
#endif

		public static int PinsCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
#if NETCOREAPP
				lock (_buckets)
#endif
				{
					var count = 0;
					for (var i = 0; i < _buckets.Length; i++)
						count += (int)(DefaultPinsBucketSize - _buckets[i].freeSlots);

					return count;
				}
			}
		}
		
		public static PinsBucket[] Buckets
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get
			{
#if NETCOREAPP
				lock (_buckets)
#endif
				{
					return _buckets;
				}
			}
		}

		private static PinsBucket[] _buckets =
		{
			new(0)
#if !DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
			, new(0)
#endif
		};

		public static PinHandle Pin(object obj)
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				if (obj == null)
					throw new ArgumentNullException(nameof(obj), "Cannot pin a null object.");

				var (bucketIndex, slotIndex) = FindEmptySlotOrResize();

				var objHash = obj.GetHashCode();
				var hash = objHash + slotIndex;

				var handle = new ObjectPin(hash, obj);

				ref var bucket = ref _buckets[bucketIndex];
				bucket.pinnedObjects[slotIndex] = handle;
				bucket.freeSlots--;

				return new PinHandle(hash, bucketIndex, slotIndex);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void* GetAddress(PinHandle pin)
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				ref var handle = ref GetRefPin(pin);
				return UnsafeUtilities.ReferenceToPointer(handle.target);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object GetTarget(PinHandle pin)
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				ref var handle = ref GetRefPin(pin);
				return handle.target;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Free(PinHandle pin)
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				ref var handle = ref GetRefPin(pin);
				handle = default;
				_buckets[pin.bucketIndex].freeSlots++;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ref ObjectPin GetRefPin(PinHandle pin)
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				ref var h = ref _buckets[pin.bucketIndex].pinnedObjects[pin.slotIndex];
				if (h.hash == pin.hash)
					return ref h;

				throw new Exception($"Failed to find handle with hash {pin.hash} in pinned objects array!");
			}
		}

		private static (short bucket, short index) FindEmptySlotOrResize()
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				for (short i = 0; i < _buckets.Length; i++)
				{
					ref var bucket = ref _buckets[i];
					if (bucket.freeSlots > 0)
					{
						var index = bucket.FastFindFreeSlot();
						if (index == -1)
							throw new Exception(
								$"Failed to find empty slot in bucket {i}! Mismatched free slots count!");

						return (i, index);
					}
				}

				var oldLength = _buckets.Length;
				var newSize = oldLength * 2;
				if (newSize > short.MaxValue)
					throw new Exception(
						$"Failed to resize buckets! New size {newSize} is greater than max value {short.MaxValue}!");

				Array.Resize(ref _buckets, newSize);
				for (var i = oldLength; i < newSize; i++)
					_buckets[i] = new PinsBucket(DefaultPinsBucketSize);

				return ((short)oldLength, 0);
			}
		}

		public static void FreeAllPins()
		{
#if NETCOREAPP
			lock (_buckets)
#endif
			{
				for (var i = 0; i < _buckets.Length; i++)
				{
					ref var bucket = ref _buckets[i];
					for (var j = 0; j < DefaultPinsBucketSize; j++)
						bucket.pinnedObjects[j] = default;

					bucket.freeSlots = DefaultPinsBucketSize;
				}
			}
		}
	}
}