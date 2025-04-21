// namespace DamnScript.Runtimes.Cores.Allocators
// {
// 	public readonly unsafe struct AllocationBucketPtr
// 	{
// 		public ref AllocationBucket RefValue => ref UnsafeUtilities.AsRef<AllocationBucket>(value);
// 		
// 		public readonly AllocationBucket* value;
// 		
// 		public AllocationBucketPtr(AllocationBucket* value)
// 		{
// 			this.value = value;
// 		}
// 		
// 		public static implicit operator AllocationBucketPtr(AllocationBucket* value) => new(value);
// 		public static implicit operator AllocationBucket*(AllocationBucketPtr value) => value.value;
// 	}
// 	
// 	public unsafe struct AllocationBucket
// 	{
// 		public AllocationBucketType Type => type;
//
// 		public byte* Data
// 		{
// 			get
// 			{
// 				fixed (byte* ptr = data)
// 					return ptr;
// 			}
// 		}
//
// 		public int TotalAllocated
// 		{
// 			get
// 			{
// 				var allocated = 0;
// 				var begin = Data;
// 				var end = begin + allocated;
// 				while (begin < end)
// 				{
// 					var header = (AllocationHeader*)begin;
// 					if (!header->isFree)
// 						allocated += header->size;
// 					
// 					begin += AllocationHeader.Size + header->size;
// 				}
//
// 				return allocated;
// 			}
// 		}
// 		
// 		public int TotalFree => dataSize - TotalAllocated;
//
// 		public AllocationBucketType type;
// 		public int dataSize;
// 		public fixed byte data[0xff];
// 		
// 		public void SetDefaultHeader()
// 		{
// 			var header = (AllocationHeader*)Data;
// 			header->isFree = true;
// 			header->size = dataSize - AllocationHeader.Size;
// 		}
// 		
// 		public void* TryAlloc(int size)
// 		{
// 			var sizeWithHeader = size + AllocationHeader.Size;
// 			
// 			var begin = Data;
// 			var end = Data + dataSize;
// 			while (begin < end)
// 			{
// 				var header = (AllocationHeader*)begin;
// 				if (header->isFree && header->size >= size)
// 				{
// 					header->isFree = false;
// 					if (header->size > size)
// 					{
// 						var newInfo = (AllocationHeader*)(begin + sizeWithHeader);
// 						newInfo->isFree = true;
// 						newInfo->size = header->size - sizeWithHeader;
// 						header->size = size;
// 					}
// 					return begin + AllocationHeader.Size;
// 				}
// 				
// 				begin += AllocationHeader.Size + header->size;
// 			}
//
// 			return null;
// 		}
//
// 		public bool TryReAlloc(void* ptr, int newSize)
// 		{
// 			var header = (AllocationHeader*)((byte*)ptr - AllocationHeader.Size);
// 			var addSize = newSize - header->size;
// 			if (addSize <= 0)
// 				return true;
// 			
// 			var nextHeader = (AllocationHeader*)((byte*)ptr + header->size);
// 			
// 			if (!IsInRange(nextHeader))
// 				return false;
// 			
// 			if (nextHeader->isFree && nextHeader->size >= addSize)
// 			{
// 				if (nextHeader->size > addSize)
// 				{
// 					var newNextHeader = (AllocationHeader*)((byte*)ptr + newSize);
// 					newNextHeader->isFree = true;
// 					newNextHeader->size = nextHeader->size - addSize - AllocationHeader.Size;
// 				}
// 				
// 				header->size = newSize;
// 				
// 				return true;
// 			}
// 			
// 			return false;
// 		}
//
// 		public void Free(void* ptr)
// 		{
// 			var header = (AllocationHeader*)((byte*)ptr - AllocationHeader.Size);
// 			header->isFree = true;
// 			Merge(header);
// 		}
// 	
// 		public bool IsInRange(void* ptr) => ptr >= Data && ptr < Data + dataSize;
// 		
// 		public AllocationHeader TryGetHeader(void* ptr)
// 		{
// 			var begin = Data;
// 			var end = Data + dataSize;
// 			while (begin < end)
// 			{
// 				var info = (AllocationHeader*)begin;
// 				var currentPtr = begin + AllocationHeader.Size;
// 				if (currentPtr == ptr)
// 					return *info;
//
// 				begin += AllocationHeader.Size + info->size;
// 			}
//
// 			return default;
// 		}
//
// 		public int TryGetOffset(void* ptr)
// 		{
// 			var begin = Data;
// 			var end = Data + dataSize;
// 			while (begin < end)
// 			{
// 				var info = (AllocationHeader*)begin;
// 				var currentPtr = begin + AllocationHeader.Size;
// 				if (currentPtr == ptr)
// 					return (int)(begin - Data);
//
// 				begin += AllocationHeader.Size + info->size;
// 			}
// 			
// 			return -1;
// 		}
//
// 		public void Merge(AllocationHeader* startHeader)
// 		{
// 			var begin = (byte*)startHeader;
// 			var end = Data + dataSize;
// 			if (begin + startHeader->size == end)
// 				return;
// 			
// 			var nextHeader = (AllocationHeader*)(begin + AllocationHeader.Size + startHeader->size);
// 			if (nextHeader->isFree)
// 			{
// 				startHeader->size += AllocationHeader.Size + nextHeader->size;
// 				nextHeader->isFree = false;
// 			}
// 		}
//
// 		public void MergeAll()
// 		{
// 			var begin = Data;
// 			var end = Data + dataSize;
// 			while (begin < end)
// 			{
// 				var currentHeader = (AllocationHeader*)begin;
// 				if (currentHeader->isFree)
// 				{
// 					var nextHeader = (AllocationHeader*)(begin + AllocationHeader.Size + currentHeader->size);
// 					while (!nextHeader->isFree)
// 					{
// 						currentHeader->size += nextHeader->size;
// 						nextHeader->isFree = false;
// 						nextHeader = (AllocationHeader*)(begin + AllocationHeader.Size + currentHeader->size);
// 						if (!IsInRange(nextHeader))
// 							return;
// 					}
// 				}
// 				begin += AllocationHeader.Size + currentHeader->size;
// 			}
// 		}
// 	}
// }