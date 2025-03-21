using System.Runtime.CompilerServices;
using System.Text;

namespace DamnScriptTests;

public class VirtualMachineTests
{
	public class TestClass
	{
		public string Value = "Test VALUE";
	}
	
	private static void Empty() { }
	private static ScriptValuePtr CreateTestClass() => ScriptValue.FromReferencePin(new TestClass()).Return();
	private static async Task Wait() => await Task.Delay(100);
	[MethodImpl(MethodImplOptions.NoOptimization)]
	private static void BigAlloc(ScriptValuePtr offset)
	{
		var before = GC.GetTotalMemory(true);
		var array = new string[100];
		for (var i = 0; i < array.Length; i++)
			array[i] = new string((char)(i % 30 + offset.IntValue), 100 + i);
		var after = GC.GetTotalMemory(true);
		Console.WriteLine($"Alloc {offset.IntValue} - {before} -> {after} = {after - before}");
		
		before = GC.GetTotalMemory(true);
		var val = new TestClass();
		after = GC.GetTotalMemory(true);
		Console.WriteLine($"Checkup {offset.IntValue} - {before} -> {after} = {after - before}");
	}
	
	private static ScriptValuePtr PrintTestClass(ScriptValuePtr value)
	{
		var test = value.GetReferencePin<TestClass>();
		Console.WriteLine(test.Value);
		return ScriptValue.FromReferenceUnsafe(test.Value).Return();
	}
	
	private static ScriptValuePtr PopStack()
	{
		return ScriptEngine.CurrentThreadPtr.RefValue.StackPop().Return();
	}
	
	[Test]
	public unsafe void ReuseScriptData()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);		
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
		Assert.That(ScriptEngine.MainPtr.RefValue.threads.Length, Is.EqualTo(16));
		ScriptEngine.UnloadScript(scriptDataPtr);
		Assert.That(scriptDataPtr.RefValue, Is.Not.EqualTo(scriptData));
	}
	
	[Test]
	public void ReAllocThreadsTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);		
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
		
		Assert.That(ScriptEngine.MainPtr.RefValue.threads.Length, Is.EqualTo(32));
		
		for (var i = 0; i < 32; i++)
			Assert.That(threads[i].Ptr.RefValue.offset, Is.EqualTo(i));
		ScriptEngine.UnloadScript(scriptData);
	}
	
	[Test]
	public void AllocWhenExecutingTest()
	{
		ScriptEngine.MainPtr.RefValue.Dispose();
		ScriptEngine.MainPtr.RefValue = new VirtualMachine(16);		
		ScriptEngine.RegisterNativeMethod(CreateTestClass);
		ScriptEngine.RegisterNativeMethod(Wait);
		ScriptEngine.RegisterNativeMethod(BigAlloc);
		ScriptEngine.RegisterNativeMethod(PrintTestClass);
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
					PrintTestClass(PopStack());
				}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptData = ScriptEngine.LoadScript(stream, "Main");
		stream.Dispose();
		var thread = ScriptEngine.RunThread(scriptData, "Main");
		while (ScriptEngine.ExecuteVirtualMachineNext())
			Thread.Sleep(10);
		ScriptEngine.UnloadScript(scriptData);
		
		Assert.That(thread.Ptr.RefValue.StackPop().GetReferenceUnsafe<string>(), Is.EqualTo("Test VALUE"));
	}
}