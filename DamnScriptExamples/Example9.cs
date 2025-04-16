using System.Text;
using DamnScript.Runtimes;
using DamnScript.Runtimes.VirtualMachines.Scripts;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptExamples
{
	public static class Example9
	{
		public const string ExampleOrigin = @"
		region Main {
			Print(""Hello from origin"");
			Swap();
			Print(""Still hello from origin"");
		}
";
		public const string ExampleNew = @"
		region Main {
			Print(""Hello from new"");
			Swap();
			Print(GetUnexpectedString());
		}
";

		public static ScriptValuePtr GetUnexpectedString() => 
			ScriptValue.FromReferencePin("WOW!!! That's not the code was loaded before").Return();
		
		public static void Swap()
		{
			using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(ExampleNew));
			ScriptsStorage.ReloadScript(memoryStream, "Example9");
		}

		public static void Run()
		{
			ScriptEngine.RegisterNativeMethod(Swap);
			ScriptEngine.RegisterNativeMethod(GetUnexpectedString);
			using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(ExampleOrigin));
			var scriptData = ScriptEngine.LoadScript(memoryStream, "Example9");
			Shared.PrintDisassembly(scriptData);
			var thread = ScriptEngine.RunThread(scriptData, "Main");
        
			Console.Write("\n");
			ScriptEngine.ExecuteVirtualMachineNext();
			Console.Write("\n");
        
			ScriptEngine.UnloadScript(scriptData);
        
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey();
		}
	}
}