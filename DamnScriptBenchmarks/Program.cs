using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.Debugs;
using MoonSharp.Interpreter;

namespace DamnScriptBenchmarks
{
    public static class Program
    {
        public static void Main()
        {
            // BenchmarkRunner.Run<PinBenchmark>();
            // BenchmarkRunner.Run<LoadLargeScriptBenchmark>();
            // var benchmark = new LoadLargeScriptBenchmark();
            // benchmark.Initialize();
            // benchmark.LuaBenchmark();
            // benchmark.DamnScriptBenchmark();
            // benchmark.CompiledScriptParserBenchmark();
        }
    }
}