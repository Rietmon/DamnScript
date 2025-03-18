using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Natives;
using MoonSharp.Interpreter;

namespace DamnScriptBenchmarks;

[Config(typeof(BenchmarksConfig))]
public class LoadLargeScriptBenchmark
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
            
	public static ScriptValue ExternalGetDamnScriptValue()
	{
		return 5;
	}
            
	public static void ExternalPrintDamnScript(ScriptValue value)
	{
		result = value.intValue;
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
		script.LoadString(@"
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								ExternalPrintLua(ExternalGetLuaValue())
								");
	}
            
	[Benchmark]
	public void DamnScriptBenchmark()
	{
		var code = @"
                region Main
                {
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                    ExternalPrintDamnScript(ExternalGetDamnScript());
                }
                ";
                
		var scriptData = ScriptEngine.LoadScript(new MemoryStream(Encoding.UTF8.GetBytes(code)), "Main");
		ScriptEngine.UnloadScript(scriptData);
	}

	[Benchmark]
	public void CompiledScriptParserBenchmark()
	{
		var scriptData = ScriptEngine.LoadCompiledScript(File.Open("/Users/rietmon/Documents/Projects/DamnScript/DamnScriptBenchmarks/bin/Release/net7.0/Main.dsc", FileMode.Open), "Main");
		
		ScriptEngine.UnloadScript(scriptData);
	}
}