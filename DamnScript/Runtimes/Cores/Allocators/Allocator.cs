// using System;
// using DamnScript.Runtimes.Cores.Collections;
//
// namespace DamnScript.Runtimes.Cores.Allocators
// {
// 	public static unsafe class Allocator
// 	{
// 		public static int TotalAllocated
// 		{
// 			get
// 			{
// 				var totalAllocated = 0;
// 				var begin = storages.Begin;
// 				var end = storages.End;
// 				while (begin < end)
// 				{
// 					if (begin->label == AllocationStorageLabel.Invalid)
// 						continue;
// 					
// 					totalAllocated += begin->TotalAllocated;
// 					begin++;
// 				}
// 				
// 				return totalAllocated;
// 			}
// 		}
//
// 		public static int TotalFree
// 		{
// 			get
// 			{
// 				var totalFree = 0;
// 				var begin = storages.Begin;
// 				var end = storages.End;
// 				while (begin < end)
// 				{
// 					if (begin->label == AllocationStorageLabel.Invalid)
// 						continue;
//
// 					totalFree += begin->TotalFree;
// 					begin++;
// 				}
// 				
// 				return totalFree;
// 			}
// 		}
// 		
// 		public static NativeList<AllocationStorage> storages = new(8);
//
// 		static Allocator()
// 		{
// 			storages.Add(new AllocationStorage(AllocationStorageLabel.DSThreads, 0, 0, 2));
// 			storages.Add(new AllocationStorage(AllocationStorageLabel.DSScripts, 0, 0, 2));
// 			storages.Add(new AllocationStorage(AllocationStorageLabel.DSStrings, 0, 2, 0));
// #if DAMN_SCRIPT_ENABLE_UNSAFE_SCRIPT_VALUE
// 			storages.Add(new AllocationStorage(AllocationStorageLabel.DSReturnValues, 12, 4, 0));
// #endif
// 			storages.Add(new AllocationStorage(AllocationStorageLabel.DSPersistentTemp, 12, 4, 1));
// 			
// 			storages.Add(new AllocationStorage(AllocationStorageLabel.Temp, 12, 4, 1));
// 		}
//
// 		public static void* Alloc(AllocationStorageLabel label, int size)
// 		{
// 			var storage = GetStorage(label).value;
// 			return storage->Alloc(size);
// 		}
//
// 		public static void Free(AllocationStorageLabel label, void* ptr)
// 		{
// 			var storage = GetStorage(label).value;
// 			storage->Free(ptr);
// 		}
//
// 		public static void AddStorage(AllocationStorage storage)
// 		{
// 			if (GetStorage(storage.label).value != null)
// 				throw new Exception($"Already allocated storage {storage.label}");
// 			
// 			storages.Add(storage);
// 		}
//
// 		public static AllocationStoragePtr GetStorage(AllocationStorageLabel label)
// 		{
// 			var begin = storages.Begin;
// 			var end = storages.End;
// 			while (begin < end)
// 			{
// 				if (begin->label == label)
// 					return new AllocationStoragePtr(begin);
// 				
// 				begin++;
// 			}
//
// 			return default;
// 		}
// 	}
// }