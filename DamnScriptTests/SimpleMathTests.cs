namespace DamnScriptTests;

public class SimpleMathTests
{
	[Test]
	public void AddTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);
		Assert.That(Run("PushToStack(5 + 5);").intValue, Is.EqualTo(10));
	}
	
	[Test]
	public void SubTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);
		Assert.That(Run("PushToStack(5 - 5);").intValue, Is.EqualTo(0));
	}
	
	[Test]
	public void MulTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);
		Assert.That(Run("PushToStack(5 * 5);").intValue, Is.EqualTo(25));
	}
	
	[Test]
	public void DivTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);
		Assert.That(Run("PushToStack(5 / 5);").intValue, Is.EqualTo(1));
	}
	
	[Test]
	public void ModTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);
		Assert.That(Run("PushToStack(5 % 5);").intValue, Is.EqualTo(0));
	}
}