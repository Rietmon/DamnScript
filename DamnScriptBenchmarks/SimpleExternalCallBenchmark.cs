using System.Text;
using BenchmarkDotNet.Attributes;
using DamnScript.Runtimes;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;
using MoonSharp.Interpreter;

namespace DamnScriptBenchmarks
{
	[Config(typeof(BenchmarksConfig))]
	public class SimpleExternalCallBenchmark
	{
		public static int result;
            
		public static int ExternalGetLuaValue()
		{
			return 5;
		}

		public static void ExternalPrintLua(int value)
		{
			result = value;
		}
            
		public static ScriptValuePtr ExternalGetDamnScriptValue()
		{
			return new ScriptValue(5).Return();
		}
            
		public static void ExternalPrintDamnScript(ScriptValuePtr value)
		{
			result = value.IntValue;
		}
            
		[GlobalSetup]
		public void Initialize()
		{
			ScriptEngine.RegisterNativeMethod(ExternalGetDamnScriptValue);
			ScriptEngine.RegisterNativeMethod(ExternalPrintDamnScript);
		}
            
		[Benchmark]
		public void LuaBenchmark()
		{
			var script = new Script();
			script.Globals["ExternalGetLuaValue"] = (Func<int>) ExternalGetLuaValue;
			script.Globals["ExternalPrintLua"] = (Action<int>) ExternalPrintLua;
			script.DoString(@"ExternalPrintLua(ExternalGetLuaValue())");
		}
            
		[Benchmark]
		public void DamnScriptBenchmark()
		{
			var code = @"
                region Main
                {
                    ExternalPrintDamnScript(ExternalGetDamnScriptValue());
                }
                ";
                
			var scriptData = ScriptEngine.LoadScript(new MemoryStream(Encoding.UTF8.GetBytes(code)), "Main");
			var thread = ScriptEngine.RunThread(scriptData, "Main");
			while (ScriptEngine.ExecuteVirtualMachineNext()) { }
			ScriptEngine.UnloadScript(scriptData);
		}
	}
}