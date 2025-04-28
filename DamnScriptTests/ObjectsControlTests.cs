using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptTests
{
	public class ObjectsControlTests
	{
		public class TestClass
		{
			public int Value { get; set; }
		
			public ScriptValuePtr Add(ScriptValuePtr value)
			{
				var thisValue = this;
				Value += value.RawInt;
				return ScriptValue.FromReferenceUnsafe(thisValue).Return();
			}
		
			public ScriptValuePtr Simulate(ScriptValuePtr value1, ScriptValuePtr value2, ScriptValuePtr value3, ScriptValuePtr value4, 
				ScriptValuePtr value5, ScriptValuePtr value6, ScriptValuePtr value7, ScriptValuePtr value8, ScriptValuePtr value9)
			{
				Value += value1.RawInt + value2.RawInt + value3.RawInt + value4.RawInt +
				         value5.RawInt + value6.RawInt + value7.RawInt + value8.RawInt + value9.RawInt;
				return ScriptValue.FromReferenceUnsafe(this).Return();
			}
		
			public async Task<ScriptValuePtr> SimulateAsync(ScriptValuePtr value1, ScriptValuePtr value2, ScriptValuePtr value3, ScriptValuePtr value4, 
				ScriptValuePtr value5, ScriptValuePtr value6, ScriptValuePtr value7, ScriptValuePtr value8, ScriptValuePtr value9)
			{
				var handle = ScriptEngine.CurrentThreadHandle;
				await Task.Delay(1);
				Value += value1.RawInt + value2.RawInt + value3.RawInt + value4.RawInt +
				         value5.RawInt + value6.RawInt + value7.RawInt + value8.RawInt + value9.RawInt;
				return ScriptValue.FromReferencePin(this).ReturnAsync(handle);
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
		
			Assert.That(Run("Simulate(Create(), 10, 20, 30, 40, 50, 60, 70, 80, 90);").GetReference<TestClass>()?.Value, Is.EqualTo(455));
			Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
		}
	
		[Test]
		public void MethodCallWithManyArgumentsAsyncTest()
		{
			ScriptEngine.mainPtr.RefValue.Dispose();
			ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);

			ScriptEngine.RegisterNativeMethod(Create);
			ScriptEngine.RegisterNativeMethod(typeof(TestClass).GetMethod(nameof(TestClass.SimulateAsync)));
		
			var result = Run("SimulateAsync(Create(), 10, 20, 30, 40, 50, 60, 70, 80, 90);");
			Assert.That(result.GetReference<TestClass>().Value, Is.EqualTo(455));
			result.UnpinSafePointer();
			Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
		}
	}
}