global using static DamnScriptTests.Shared;
global using DamnScript.Runtimes;
global using DamnScript.Runtimes.Cores;
global using DamnScript.Runtimes.Cores.Types;
global using DamnScript.Runtimes.Debugs;
global using DamnScript.Runtimes.Metadatas;
global using DamnScript.Runtimes.Natives;
global using DamnScript.Runtimes.Serializations;
global using DamnScript.Runtimes.VirtualMachines;
global using DamnScript.Runtimes.VirtualMachines.Datas;
global using DamnScript.Runtimes.VirtualMachines.Threads;

using System.Text;
using DamnScript.Runtimes.Cores.Pins;

namespace DamnScriptTests;

public static class Shared
{
	public static void PushToStack(ScriptValuePtr value)
	{
		ScriptEngine.CurrentThreadPtr.RefValue.StackPush(value.LongValue);
	}
	
	public static void Print(ScriptValuePtr value)
	{
		var str = value.RefValue.ToString();
		Console.WriteLine(str);
	}

	public static ScriptValue Run(string method)
	{
		ScriptEngine.RegisterNativeMethod(PushToStack);
		ScriptEngine.RegisterNativeMethod(Print);
		
		var code = $@"
				region Main
				{{
					{method}
				}}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptData = ScriptEngine.LoadScript(stream, "Main");
		stream.Dispose();
		var thread = ScriptEngine.RunThread(scriptData, "Main");
		while (ScriptEngine.ExecuteVirtualMachineNext())
		{
			Assert.That(thread.Ptr.RefValue.awaitTaskPin.hash, Is.Not.EqualTo(0));
			Thread.Sleep(10);
		}
		Assert.That(thread.Ptr.RefValue.isAlive, Is.False);
		ScriptEngine.UnloadScript(scriptData);
		var value = thread.Ptr.RefValue.StackPop();
		Assert.That(thread.Ptr.RefValue.stack.stackOffset, Is.EqualTo(0));
		return value;
	}
}