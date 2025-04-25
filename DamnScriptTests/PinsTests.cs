using DamnScript.Runtimes.Cores.Pins;

namespace DamnScriptTests;

public class PinsTests
{
	[Test]
	public void FreeSlotsTest()
	{
		var pins = new PinHandle[PinHelper.DefaultPinsBucketSize];
		for (var i = 0; i < PinHelper.DefaultPinsBucketSize; i++)
		{
			var toPin = new object();
			var pin = pins[i] = PinHelper.Pin(toPin);
			Assert.That(pin.Target, Is.EqualTo(toPin));
			Assert.That(pin.bucketIndex, Is.EqualTo(0));
			Assert.That(pin.slotIndex, Is.EqualTo(i));
		}
		
		Assert.That(PinHelper.PinsCount, Is.EqualTo(PinHelper.DefaultPinsBucketSize));
		Assert.That(PinHelper.Buckets[0].freeSlots, Is.EqualTo(0));
		
		for (var i = PinHelper.DefaultPinsBucketSize - 1; i >= 0; i--)
		{
			var pin = pins[i];
			Assert.That(pin.Target, Is.Not.Null);
			
			pin.Free();

			Assert.That(PinHelper.Buckets[0].FastFindFreeSlot(), Is.EqualTo(i));
		}
	}
}