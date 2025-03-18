namespace DamnScriptTests;

public class SimpleMathTests
{
	[Test]
	public void AddTest()
	{
		Assert.That(Run("PushToStack(5 + 5);").intValue, Is.EqualTo(10));
	}
	
	[Test]
	public void SubTest()
	{
		Assert.That(Run("PushToStack(5 - 5);").intValue, Is.EqualTo(0));
	}
	
	[Test]
	public void MulTest()
	{
		Assert.That(Run("PushToStack(5 * 5);").intValue, Is.EqualTo(25));
	}
	
	[Test]
	public void DivTest()
	{
		Assert.That(Run("PushToStack(5 / 5);").intValue, Is.EqualTo(1));
	}
	
	[Test]
	public void ModTest()
	{
		Assert.That(Run("PushToStack(5 % 5);").intValue, Is.EqualTo(0));
	}
}