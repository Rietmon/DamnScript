using System.Text;

namespace DamnScriptTests;

public unsafe class VirtualMachineTests
{
	private static void Empty() { }
	
	[Test]
	public void ReuseScriptData()
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
	}
}