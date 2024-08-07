using System.Text;
using DamnScript.Parsings.Serializations;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Natives;
using DamnScript.Runtimes.VirtualMachines.Threads;

namespace DamnScriptExamples
{
	public static class Example8
	{
		private const string Code = @"
		region Main 
		{
			Print(1);
			Print(""Not shutdown the application"");
			MakeSavePoint();
			Await(5000);
			Print(2);
		}
";

		private static unsafe void MakeSavePoint()
		{
			var thread = ScriptEngine.GetCurrentThread();
			thread.RefValue.ExecuteSetSavePoint();
			var data = ScriptEngine.SerializeToSerializationStream();
			var stream = File.Open("Example8.save", FileMode.OpenOrCreate);
			stream.Write(new ReadOnlySpan<byte>(data.start, data.length));
			stream.Flush();
		}
		
		private static async Task Await(ScriptValue ms)
		{
			await Task.Delay(ms.intValue);
		}
        
		public static unsafe void Run()
		{
			ScriptEngine.RegisterNativeMethod(MakeSavePoint);
			ScriptEngine.RegisterNativeMethod(Await);
			
			var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(Code));
			var scriptData = ScriptEngine.LoadScript(memoryStream, "Example8");
			Shared.PrintDisassembly(scriptData);
			if (File.Exists("Example8.save"))
			{
				var bytes = File.ReadAllBytes("Example8.save");
				fixed (byte* ptr = bytes)
				{
					var stream = new SerializationStream(ptr, bytes.Length);
					ScriptEngine.DeserializeFromSerializationStream(stream);
				}
			}
			else
			{
				ScriptEngine.RunThread(scriptData, "Main");
			}

			Console.Write("\n");
			while (ScriptEngine.ExecuteScheduler())
				Thread.Sleep(15);
			Console.Write("\n");
        
			ScriptEngine.UnloadScript(scriptData);
        
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}