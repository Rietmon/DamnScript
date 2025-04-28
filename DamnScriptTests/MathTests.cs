namespace DamnScriptTests
{
	public class MathTests
	{
		[Test]
		public void AddTest()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
			Assert.That(Run("PushToStack(5 + 5);").rawInt, Is.EqualTo(10));
		}
	
		[Test]
		public void SubTest()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
			Assert.That(Run("PushToStack(5 - 5);").rawInt, Is.EqualTo(0));
		}
	
		[Test]
		public void MulTest()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
			Assert.That(Run("PushToStack(5 * 5);").rawInt, Is.EqualTo(25));
		}
	
		[Test]
		public void DivTest()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
			Assert.That(Run("PushToStack(5 / 5);").rawInt, Is.EqualTo(1));
		}
	
		[Test]
		public void ModTest()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
			Assert.That(Run("PushToStack(5 % 5);").rawInt, Is.EqualTo(0));
		}
	
		[Test]
		public void Diff1Test()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
			Assert.That(Run("PushToStack(5 + 5 - 5 * (2 - 3 * 124) + 34 - 5);").rawInt, Is.EqualTo(1889));
		}
	}
}