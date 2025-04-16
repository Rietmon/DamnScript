using System;
using DamnScript.Runtimes.Cores.Collections;

namespace DamnScript.Runtimes.Cores.Allocators
{
	public readonly unsafe struct AllocationStoragePtr
	{
		public ref AllocationStorage RefValue => ref UnsafeUtilities.AsRef<AllocationStorage>(value);
		
		public readonly AllocationStorage* value;

		public AllocationStoragePtr(AllocationStorage* value)
		{
			this.value = value;
		}
		
		public static implicit operator AllocationStoragePtr(AllocationStorage* value) => new(value);
		public static implicit operator AllocationStorage*(AllocationStoragePtr value) => value.value;
	}
	
	public unsafe struct AllocationStorage : IDisposable
	{
		public bool IsAlive => label != AllocationStorageLabel.Invalid;

		public int TotalAllocated
		{
			get
			{
				var allocated = 0;
				var begin = buckets.Begin;
				var end = buckets.End;
				while (begin < end)
				{
					allocated += begin->value->TotalAllocated;
					begin++;
				}
				return allocated;
			}
		}

		public int TotalFree
		{
			get
			{
				var free = 0;
				var begin = buckets.Begin;
				var end = buckets.End;
				while (begin < end)
				{
					free += begin->value->TotalFree;
					begin++;
				}
				return free;
			}
		}
		
		public readonly AllocationStorageLabel label;
		
		public NativeArray<AllocationBucketPtr> buckets;

		public AllocationStorage(AllocationStorageLabel label, int smallBucketsCount, int mediumBucketsCount, int largeBucketsCount)
		{
			this.label = label;
			
			buckets = new NativeArray<AllocationBucketPtr>(smallBucketsCount + mediumBucketsCount + largeBucketsCount);
			for (var i = 0; i < smallBucketsCount; i++)
				buckets[i] = AllocateBucket(AllocationBucketType.Small);
			for (var i = 0; i < mediumBucketsCount; i++)
				buckets[smallBucketsCount + i] = AllocateBucket(AllocationBucketType.Medium);
			for (var i = 0; i < largeBucketsCount; i++)
				buckets[smallBucketsCount + mediumBucketsCount + i] = AllocateBucket(AllocationBucketType.Large);
		}

		public void* Alloc(int size)
		{
			if (size <= 0)
				throw new ArgumentOutOfRangeException(nameof(size), size, "Size must be greater than 0!");
			
			size = AlignSize(size);

			var type = GetBucketType(size);
			
			var ptr = FindBucketAndAlloc(size, 0, type);
			if (ptr != null)
				return ptr;
				
			var oldSize = buckets.Length;
			var count = GetBucketIncrement(type);
			var newSize = oldSize + count;
			fixed (NativeArray<AllocationBucketPtr>* bucketsPtr = &buckets)
				NativeArray<AllocationBucketPtr>.ReAlloc(bucketsPtr, newSize);
			
			for (var i = oldSize; i < newSize; i++)
				buckets[i] = AllocateBucket(type);
			
			ptr = FindBucketAndAlloc(size, oldSize, type);
			if (ptr != null)
				return ptr;
			
			throw new Exception($"Failed to allocate {size} bytes!");
		}

		public void* FindBucketAndAlloc(int size, int startIndex, AllocationBucketType type)
		{
			for (var i = startIndex; i < buckets.Length; i++)
			{
				var bucket = (buckets.Begin + i)->value;
				if (bucket->type != type)
					continue;
				
				var ptr = bucket->TryAlloc(size);
				if (ptr != null)
					return ptr;
			}

			return null;
		}

		public void* ReAlloc(void* ptr, int newSize)
		{
			if (newSize <= 0)
				throw new ArgumentOutOfRangeException(nameof(newSize), newSize, "NewSize must be greater than 0!");
			
			var (bucket, header) = GetBucketAndHeader(ptr);
			if (header.size >= newSize)
				return ptr;
			
			if (bucket.value->TryReAlloc(ptr, newSize))
				return ptr;
			
			Free(ptr);
			return Alloc(newSize);
		}
		
		public void Free(void* ptr)
		{
			for (var i = 0; i < buckets.Length; i++)
			{
				var bucket = (buckets.Begin + i)->value;
				if (!bucket->IsInRange(ptr))
					continue;
				
				bucket->Free(ptr);
				return;
			}
			
			throw new Exception($"Pointer {new IntPtr(ptr)} not found in any bucket!");
		}

		public AllocationBucketPtr GetBucket(void* ptr)
		{
			for (var i = 0; i < buckets.Length; i++)
			{
				var bucket = (buckets.Begin + i)->value;
				if (bucket->IsInRange(ptr))
					return bucket;
			}
				
			throw new Exception($"Pointer {new IntPtr(ptr)} not found in any bucket!");
		}
		
		public (AllocationBucketPtr bucket, AllocationHeader header) GetBucketAndHeader(void* ptr)
		{
			for (var i = 0; i < buckets.Length; i++)
			{
				var bucket = (buckets.Begin + i)->value;
				var header = bucket->TryGetHeader(ptr);
				if (header.size != 0)
					return (new AllocationBucketPtr(bucket), header);
			}
				
			throw new Exception($"Pointer {new IntPtr(ptr)} not found in any bucket!");
		}
		
		public int GetOffsetInBucket(void* ptr)
		{
			for (var i = 0; i < buckets.Length; i++)
			{
				var bucket = (buckets.Begin + i)->value;
				var offset = bucket->TryGetOffset(ptr);
				if (offset != -1)
					return offset;
			}
				
			throw new Exception($"Pointer {new IntPtr(ptr)} not found in any bucket!");
		}

		public void Dispose()
		{
			for (var i = 0; i < buckets.Length; i++)
			{
				var bucket = (buckets.Begin + i)->value;
				UnsafeUtilities.Free(bucket);
			}
			buckets.Dispose();
		}

		public static int AlignSize(int size)
		{
			const int align = 8;
			var modulo = size % align;
			if (modulo != 0)
				size += align - modulo;

			return size;
		}
		
		public static AllocationBucket* AllocateBucket(AllocationBucketType type)
		{
			const int smallDataSize = 256;
			const int smallPotentialAllocations = 8;
			const int mediumDataSize = 1024;
			const int mediumPotentialAllocations = 16;
			const int largeDataSize = 16384;
			const int largePotentialAllocations = 64;
			var size = type switch
			{
				AllocationBucketType.Small => smallDataSize + AllocationHeader.Size * smallPotentialAllocations,
				AllocationBucketType.Medium => mediumDataSize + AllocationHeader.Size * mediumPotentialAllocations,
				AllocationBucketType.Large => largeDataSize + AllocationHeader.Size * largePotentialAllocations,
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
			
			var bucket = (AllocationBucket*)UnsafeUtilities.Alloc(size);
			bucket->type = type;
			bucket->dataSize = size;
			bucket->SetDefaultHeader();
			return bucket;
		}

		public static AllocationBucketType GetBucketType(int size) =>
			size switch
			{
				<= 64 => AllocationBucketType.Small,
				<= 256 => AllocationBucketType.Medium,
				_ => AllocationBucketType.Large
			};
		
		public static int GetBucketIncrement(AllocationBucketType type) =>
			type switch
			{
				AllocationBucketType.Small => 2,
				AllocationBucketType.Medium => 1,
				AllocationBucketType.Large => 1,
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
	}
}