using DamnScript.Runtimes.Cores.Allocators;

namespace DamnScriptTests;

public unsafe class AllocatorTests
{
	[Test]
	public void AllocOffsetsTest()
	{
		var storage = new AllocationStorage(1, 0, 0);
		
		var ptr1 = storage.Alloc(64);
		var ptr2 = storage.Alloc(64);

		Assert.That(storage.GetOffsetInBucket(ptr1), Is.EqualTo(0));
		Assert.That(storage.GetOffsetInBucket(ptr2), Is.EqualTo(72));
		
		storage.Free(ptr1);
		
		var ptr3 = storage.Alloc(64);
		Assert.That(storage.GetOffsetInBucket(ptr3), Is.EqualTo(0));
		
		storage.Free(ptr2);
		storage.Free(ptr3);
		
		var ptr4 = storage.Alloc(64);
		var ptr5 = storage.Alloc(64);
		Assert.That(storage.GetOffsetInBucket(ptr4), Is.EqualTo(0));
		Assert.That(storage.GetOffsetInBucket(ptr5), Is.EqualTo(72));
		
		storage.Free(ptr5);
		
		var ptr6 = storage.Alloc(64);
		Assert.That(storage.GetOffsetInBucket(ptr6), Is.EqualTo(72));
	}

	[Test]
	public void AllocInFreedSpaceTest()
	{
		
	}
}