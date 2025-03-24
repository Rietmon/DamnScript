using System.Runtime.CompilerServices;
using System.Text;
using DamnScript.Runtimes.Cores.Pins;

namespace DamnScriptTests;

public class VirtualMachineTests
{
	private static void Empty() { }
	
	[Test]
	public unsafe void ReuseScriptData()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);		
		ScriptEngine.RegisterNativeMethod(Empty);
		
		var code = @"
				region Main
				{
					Empty();
				}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptDataPtr = ScriptEngine.LoadScript(stream, "Main");
		var scriptData = scriptDataPtr.RefValue;
		stream.Dispose();
		for (var i = 0; i < 16; i++)
		{
			var thread = ScriptEngine.RunThread(scriptDataPtr, "Main");
			Assert.That((IntPtr)thread.Ptr.RefValue.regionData, Is.EqualTo((IntPtr)scriptDataPtr.value->regions.Begin));
			while (ScriptEngine.ExecuteVirtualMachineNext())
			{
				Assert.That(thread.Ptr.RefValue.awaitTaskPin.hash, Is.Not.EqualTo(0));
				Thread.Sleep(10);
			}
			Assert.That(thread.Ptr.RefValue.stack.stackOffset, Is.EqualTo(0));
			Assert.That(thread.Ptr.RefValue.isAlive, Is.False);
		}
		Assert.That(ScriptEngine.mainPtr.RefValue.threads.Length, Is.EqualTo(16));
		ScriptEngine.UnloadScript(scriptDataPtr);
		Assert.That(scriptDataPtr.RefValue, Is.Not.EqualTo(scriptData));
		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
	
	[Test]
	public void ReAllocThreadsTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);		
		ScriptEngine.RegisterNativeMethod(Empty);
		
		var code = @"
				region Main
				{	
					Empty();
				}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptData = ScriptEngine.LoadScript(stream, "Main");
		stream.Dispose();
		var threads = new VirtualMachineThreadHandle[32];
		for (var i = 0; i < 32; i++)
		{
			threads[i] = ScriptEngine.RunThread(scriptData, "Main");
			threads[i].Ptr.RefValue.offset = i;
		}
		
		Assert.That(ScriptEngine.mainPtr.RefValue.threads.Length, Is.EqualTo(32));
		
		for (var i = 0; i < 32; i++)
			Assert.That(threads[i].Ptr.RefValue.offset, Is.EqualTo(i));
		ScriptEngine.UnloadScript(scriptData);
		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
	
	public class TestClass { public string Value = "Test VALUE"; }
	private static ScriptValuePtr CreateTestClass() => ScriptValue.FromReferencePin(new TestClass()).Return();
	private static async Task Wait() => await Task.Delay(10);

	private const int Allocs = 100;
	private static string[][] _temp = new string[10][];
	
	[MethodImpl(MethodImplOptions.NoOptimization)]
	private static unsafe void BigAlloc(ScriptValuePtr offset)
	{
		if (offset.IntValue >= 10)
		{
			if (offset.IntValue == 10)
				_temp = new string[10][];
			
			GC.Collect();
		}
		
		var before = GC.GetTotalMemory(false);
		var array = _temp[offset.IntValue % 10] = new string[Allocs * (offset.IntValue / 2)];
		for (var i = 0; i < array.Length; i++)
			array[i] = new string((char)(i % 30 + offset.IntValue), Allocs + i);

		Console.WriteLine(array);
		var after = GC.GetTotalMemory(false);
		Console.WriteLine($"Alloc {offset.IntValue} - {before} -> {after} = {after - before}");
		
		before = GC.GetTotalMemory(false);
		_ = new TestClass();
		after = GC.GetTotalMemory(false);
		Console.WriteLine($"Checkup {offset.IntValue} - {before} -> {after} = {after - before}");
	}

	
	private static async Task<ScriptValuePtr> PrintTestClassAndAlloc(ScriptValuePtr value)
	{
		await Task.Delay(10);
		for (var i = 0; i < 8; i++)
		{
			ScriptValue ret = default;
			BigAlloc(new ScriptValuePtr(ref ret));
			await Task.Delay(10);
		}
		var test = value.GetReferencePin<TestClass>();
		Console.WriteLine(test.Value);
		return ScriptValue.FromReferencePin(test.Value).Return();
	}
	private static ScriptValuePtr PopStack()
	{
		return ScriptEngine.CurrentThreadPtr.RefValue.StackPop().Return();
	}
	
	[Test]
	public unsafe void AllocWhenExecutingTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);		
		ScriptEngine.RegisterNativeMethod(CreateTestClass);
		ScriptEngine.RegisterNativeMethod(Wait);
		ScriptEngine.RegisterNativeMethod(BigAlloc);
		ScriptEngine.RegisterNativeMethod(PrintTestClassAndAlloc);
		ScriptEngine.RegisterNativeMethod(PopStack);
		
		var code = @"
				region Main
				{
					CreateTestClass();
					BigAlloc(0);
					Wait();
					BigAlloc(1);
					Wait();
					BigAlloc(2);
					for (i in 20)
					{
						BigAlloc(i);
						Wait();
					}
					PrintTestClassAndAlloc(PopStack());
				}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptData = ScriptEngine.LoadScript(stream, "Main");
		stream.Dispose();
		var thread = ScriptEngine.RunThread(scriptData, "Main");
		while (ScriptEngine.ExecuteVirtualMachineNext())
			Thread.Sleep(10);
		ScriptEngine.UnloadScript(scriptData);
		
		Assert.That(thread.Ptr.value->StackPop().GetReferencePin<string>(), Is.EqualTo("Test VALUE"));
		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
	
	public static async Task<ScriptValuePtr> UseAsyncPrimitive()
	{
		await Task.Delay(10);
		var val = new ScriptValue(12);
		await Task.Delay(10);
		return val.Return();
	}
	
	public static async Task<ScriptValuePtr> UseAsyncRef()
	{
		await Task.Delay(10);
		var val = ScriptValue.FromReferenceUnsafe(new TestClass());
		await Task.Delay(10);
		return val.Return();
	}

	[Test]
	public void UseAsyncWithoutPinTest()
	{
		ScriptEngine.mainPtr.RefValue.Dispose();
		ScriptEngine.mainPtr.RefValue = new VirtualMachine(16);		
		ScriptEngine.RegisterNativeMethod(UseAsyncPrimitive);
		ScriptEngine.RegisterNativeMethod(UseAsyncRef);
		ScriptEngine.RegisterNativeMethod(Print);
		
		var code = @"
				region Main
				{
					Print(UseAsyncPrimitive());
					Print(UseAsyncRef());
				}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptData = ScriptEngine.LoadScript(stream, "Main");
		stream.Dispose();
		ScriptEngine.RunThread(scriptData, "Main");
		var asyncCount = 0;
		try
		{
			while (ScriptEngine.ExecuteVirtualMachineNext())
			{
				asyncCount++;
				Thread.Sleep(100);
			}
		}
		catch (Exception e)
		{
			Assert.That(e.Message, Does.EndWith("DAMN_SCRIPT_DISABLE_ASYNC_PINNING."));
			Assert.That(asyncCount, Is.EqualTo(2));
		}

		Assert.That(PinHelper.PinsCount, Is.EqualTo(0));
	}
}