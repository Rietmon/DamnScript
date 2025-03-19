namespace DamnScriptTests;

public class ObjectsControlTests
{
	public class TestClass
	{
		public int Value { get; set; }
		
		public ScriptValue Add(ScriptValue value) 
		{
			Value += value.intValue;
			return ScriptValue.FromReferencePin(this);
		}
		
		public ScriptValue Simulate(ScriptValue value1, ScriptValue value2, ScriptValue value3, ScriptValue value4, 
			ScriptValue value5, ScriptValue value6, ScriptValue value7, ScriptValue value8, ScriptValue value9)
		{
			Value += value1.intValue + value2.intValue + value3.intValue + value4.intValue +
				value5.intValue + value6.intValue + value7.intValue + value8.intValue + value9.intValue;
			return ScriptValue.FromReferencePin(this);
		}
		
		public async Task<ScriptValue> SimulateAsync(ScriptValue value1, ScriptValue value2, ScriptValue value3, ScriptValue value4, 
			ScriptValue value5, ScriptValue value6, ScriptValue value7, ScriptValue value8, ScriptValue value9)
		{
			await Task.Delay(100);
			Value += value1.intValue + value2.intValue + value3.intValue + value4.intValue +
				value5.intValue + value6.intValue + value7.intValue + value8.intValue + value9.intValue;
			return ScriptValue.FromReferencePin(this);
		}
	}
	
	private static ScriptValue Create() => ScriptValue.FromReferencePin(new TestClass() { Value = 5 });
	
	[Test]
	public void CreationAndGetTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);

		ScriptEngine.RegisterNativeMethod(Create);
		
		Assert.That(Run("Create();").GetReferencePin<TestClass>().Value, Is.EqualTo(5));
	}
	
	[Test]
	public void MethodCallTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);
		
		ScriptEngine.RegisterNativeMethod(Create);
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.Add)));
		
		Assert.That(Run("Add(Create(), 10);").GetReferencePin<TestClass>()?.Value, Is.EqualTo(15));
	}
	
	[Test]
	public void MethodCallWithManyArgumentsTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);

		ScriptEngine.RegisterNativeMethod(Create);
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.Simulate)));
		
		Assert.That(Run("Simulate(Create(), 1, 2, 3, 4, 5, 6, 7, 8, 9);").GetReferencePin<TestClass>()?.Value, Is.EqualTo(50));
	}
	
	[Test]
	public void MethodCallWithManyArgumentsAsyncTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);

		ScriptEngine.RegisterNativeMethod(Create);
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.SimulateAsync)));
		
		Assert.That(Run("SimulateAsync(Create(), 1, 2, 3, 4, 5, 6, 7, 8, 9);").GetReferencePin<TestClass>().Value, Is.EqualTo(50));
	}
}