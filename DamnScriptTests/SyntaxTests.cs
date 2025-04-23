using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptTests;

public class SyntaxTests
{
	[Test]
	public void IfTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

		Assert.That(Run("if (1) { PushToStack(1); } else { PushToStack(2); }").intValue, Is.EqualTo(1));
		Assert.That(Run("if (0) { PushToStack(1); } else { PushToStack(2); }").intValue, Is.EqualTo(2));
		Assert.That(Run("if (1) { if (1) { PushToStack(1); } else { PushToStack(2); } } else { PushToStack(3); }").intValue, Is.EqualTo(1));
		Assert.That(Run("if (0) { if (1) { PushToStack(1); } else { PushToStack(2); } } else { PushToStack(3); }").intValue, Is.EqualTo(3));
	}
	
	private static int _counter;
	public static ScriptValuePtr GetCounter() => (_counter++).Return();
	
	[Test]
	public void WhileTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
		
		ScriptEngine.RegisterNativeMethod(GetCounter);

		Assert.That(Run("while (GetCounter() < 5) { PushToStack(GetCounter()); } ", true).intValue, Is.EqualTo(5));
		_counter = 0;
		Assert.That(Run("while (GetCounter() < 10) { PushToStack(GetCounter()); }", true).intValue, Is.EqualTo(9));
		_counter = 0;
		Assert.That(Run("while (GetCounter() < 15) { PushToStack(GetCounter()); } ", true).intValue, Is.EqualTo(15));
		_counter = 0;
	}

	public static ScriptValuePtr GetUntil() => 15.Return();
	
	[Test]
	public void ForTest()
	{
		ScriptEngine.RegisterNativeMethod(GetUntil);
		
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
		
		Assert.That(Run("for (i in 5) { PushToStack(i); } ", true).intValue, Is.EqualTo(4));
		Assert.That(Run("for (i in 10) { PushToStack(i); }", true).intValue, Is.EqualTo(9));
		Assert.That(Run("for (i in 20) { PushToStack(i); }", true).intValue, Is.EqualTo(19));
		Assert.That(Run("for (i in 20 + 1) { PushToStack(i); }", true).intValue, Is.EqualTo(20));
		Assert.That(Run("for (i in GetUntil()) { PushToStack(i); }", true).intValue, Is.EqualTo(14));
		Assert.That(Run("for (i in GetUntil() + 1) { PushToStack(i); }", true).intValue, Is.EqualTo(15));
		const string code = @"
for (i in 1) {
	for (j in 1) {
		for (k in 1) {
			for (l in 1) {		
				PushToStack(1);
			}
		}
	}
}
";
		Assert.That(Run(code, true).intValue, Is.EqualTo(1));
	}
}