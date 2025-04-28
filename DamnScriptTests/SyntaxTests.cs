using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptTests;

public class SyntaxTests
{
	[Test]
	public void IfTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

		Assert.That(Run("if (1) { PushToStack(1); } else { PushToStack(2); }").rawInt, Is.EqualTo(1));
		Assert.That(Run("if (0) { PushToStack(1); } else { PushToStack(2); }").rawInt, Is.EqualTo(2));
		Assert.That(Run("if (1) { if (1) { PushToStack(1); } else { PushToStack(2); } } else { PushToStack(3); }").rawInt, Is.EqualTo(1));
		Assert.That(Run("if (0) { if (1) { PushToStack(1); } else { PushToStack(2); } } else { PushToStack(3); }").rawInt, Is.EqualTo(3));
	}
	
	private static int _counter;
	public static ScriptValuePtr GetCounter() => (_counter++).Return();
	
	[Test]
	public void WhileTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
		
		ScriptEngine.RegisterNativeMethod(GetCounter);

		Assert.That(Run("while (GetCounter() < 5) { PushToStack(GetCounter()); } ", true).rawInt, Is.EqualTo(5));
		_counter = 0;
		Assert.That(Run("while (GetCounter() < 10) { PushToStack(GetCounter()); }", true).rawInt, Is.EqualTo(9));
		_counter = 0;
		Assert.That(Run("while (GetCounter() < 15) { PushToStack(GetCounter()); } ", true).rawInt, Is.EqualTo(15));
		_counter = 0;
	}

	public static ScriptValuePtr GetUntil() => 8.Return();
	
	[Test]
	public void ForTest()
	{
		ScriptEngine.RegisterNativeMethod(GetUntil);
		
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
		
		Assert.That(Run("for (i in 5) { PushToStack(i); } ", true).rawInt, Is.EqualTo(4));
		Assert.That(Run("for (i in 10) { PushToStack(i); }", true).rawInt, Is.EqualTo(9));
		Assert.That(Run("for (i in 12) { PushToStack(i); }", true).rawInt, Is.EqualTo(11));
		Assert.That(Run("for (i in 12 + 1) { PushToStack(i); }", true).rawInt, Is.EqualTo(12));
		Assert.That(Run("for (i in GetUntil()) { PushToStack(i); }", true).rawInt, Is.EqualTo(7));
		Assert.That(Run("for (i in GetUntil() + 1) { PushToStack(i); }", true).rawInt, Is.EqualTo(8));
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
		Assert.That(Run(code, true).rawInt, Is.EqualTo(1));
	}

	public class TestClass
	{
		public ScriptValuePtr GetStr() => ScriptValue.FromReferencePin("Hello").Return();
		public ScriptValuePtr This() => ScriptValue.FromReferencePin(this).Return();
		
		public ScriptValuePtr GetStrU() => ScriptValue.FromReferenceUnsafe("Hello").Return();
		public ScriptValuePtr ThisU() => ScriptValue.FromReferenceUnsafe(this).Return();
	}

	public static ScriptValuePtr NewClass() => ScriptValue.FromReferencePin(new TestClass()).Return();
	public static ScriptValuePtr NewClassU() => ScriptValue.FromReferenceUnsafe(new TestClass()).Return();

	[Test]
	public void TestObjectCalls()
	{
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod("GetStr"));
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod("This"));
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod("GetStrU"));
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod("ThisU"));
		ScriptEngine.RegisterNativeMethod(NewClass);
		ScriptEngine.RegisterNativeMethod(NewClassU);
		
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

		var safeValue = Run("NewClass().GetStr();");
		var value = safeValue.GetReference<string>();
		safeValue.UnpinSafePointer();
		Assert.That(value, Is.EqualTo("Hello"));
		
		safeValue = Run("NewClass().This().GetStr();");
		value = safeValue.GetReference<string>();
		safeValue.UnpinSafePointer();
		Assert.That(value, Is.EqualTo("Hello"));
		
		safeValue = Run("NewClass().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().This().GetStr();");
		value = safeValue.GetReference<string>();
		safeValue.UnpinSafePointer();
		Assert.That(value, Is.EqualTo("Hello"));

		safeValue = Run("NewClassU().GetStrU();");
		value = safeValue.GetReference<string>();
		Assert.That(value, Is.EqualTo("Hello"));
		
		safeValue = Run("NewClassU().ThisU().GetStrU();");
		value = safeValue.GetReference<string>();
		Assert.That(value, Is.EqualTo("Hello"));
		
		safeValue = Run("NewClassU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().ThisU().GetStrU();");
		value = safeValue.GetReference<string>();
		Assert.That(value, Is.EqualTo("Hello"));
	}
}