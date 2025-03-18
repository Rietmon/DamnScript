using System.Text;

namespace DamnScriptTests;

public class VirtualMachineTests
{
	public static void Empty() { }
	
	[Test]
	public void ReuseScriptData()
	{
		var code = @"
				region Main
				{
					Empty();
				}
				";
		
		var stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
		var scriptData = ScriptEngine.LoadScript(stream, "Main");
		stream.Dispose();
		for (var i = 0; i < 16; i++)
		{
			var thread = ScriptEngine.RunThread(scriptData, "Main");
			while (ScriptEngine.ExecuteVirtualMachineNext()) 
				Thread.Sleep(10);
			Assert.That(thread.RefValue.stack.stackOffset, Is.EqualTo(0));
			Assert.That(thread.RefValue.isAlive, Is.False);
		}
	}
}