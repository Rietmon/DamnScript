using DamnScript.Runtimes.Cores.Pins;

namespace DamnScriptTests;

public class ObjectsControlTests
{
	public class TestClass
	{
		public int Value { get; set; }
		
		public ScriptValuePtr Add(ScriptValuePtr value)
		{
			var thisValue = this;
			Value += value.IntValue;
			return ScriptValue.FromReferenceUnsafe(thisValue).Return();
		}
		
		public ScriptValuePtr Simulate(ScriptValuePtr value1, ScriptValuePtr value2, ScriptValuePtr value3, ScriptValuePtr value4, 
			ScriptValuePtr value5, ScriptValuePtr value6, ScriptValuePtr value7, ScriptValuePtr value8, ScriptValuePtr value9)
		{
			Value += value1.IntValue + value2.IntValue + value3.IntValue + value4.IntValue +
				value5.IntValue + value6.IntValue + value7.IntValue + value8.IntValue + value9.IntValue;
			return ScriptValue.FromReferenceUnsafe(this).Return();
		}
		
		public async Task<ScriptValuePtr> SimulateAsync(ScriptValuePtr value1, ScriptValuePtr value2, ScriptValuePtr value3, ScriptValuePtr value4, 
			ScriptValuePtr value5, ScriptValuePtr value6, ScriptValuePtr value7, ScriptValuePtr value8, ScriptValuePtr value9)
		{
			await Task.Delay(100);
			Value += value1.IntValue + value2.IntValue + value3.IntValue + value4.IntValue +
			         value5.IntValue + value6.IntValue + value7.IntValue + value8.IntValue + value9.IntValue;
			return ScriptValue.FromReferencePin(this).Return();
		}
	}
	
	private static ScriptValuePtr Create() => ScriptValue.FromReferenceUnsafe(new TestClass() { Value = 5 }).Return();
	
	[Test]
	public void CreationAndGetTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

		ScriptEngine.RegisterNativeMethod(Create);
		
		Assert.That(Run("Create();").GetReference<TestClass>().Value, Is.EqualTo(5));
	}
	
	[Test]
	public void MethodCallTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);
		
		ScriptEngine.RegisterNativeMethod(Create);
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.Add)));
		
		Assert.That(Run("Add(Create(), 10);").GetReference<TestClass>()?.Value, Is.EqualTo(15));
		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
	
	[Test]
	public void MethodCallWithManyArgumentsTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

		ScriptEngine.RegisterNativeMethod(Create);
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.Simulate)));
		
		Assert.That(Run("Simulate(Create(), 1, 2, 3, 4, 5, 6, 7, 8, 9);").GetReference<TestClass>()?.Value, Is.EqualTo(50));
		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
	
	[Test]
	public void MethodCallWithManyArgumentsAsyncTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

		ScriptEngine.RegisterNativeMethod(Create);
		ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.SimulateAsync)));
		
		Assert.That(Run("SimulateAsync(Create(), 1, 2, 3, 4, 5, 6, 7, 8, 9);").GetReference<TestClass>().Value, Is.EqualTo(50));
		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
}