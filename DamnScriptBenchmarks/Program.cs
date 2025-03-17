using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using DamnScript.Parsings.Compilings;
using DamnScript.Runtimes;
using DamnScript.Runtimes.Cores;
using DamnScript.Runtimes.Debugs;
using DamnScript.Runtimes.Natives;
using MoonSharp.Interpreter;

namespace DamnScriptBenchmarks
{
    public static unsafe class Program
    {
        public static void Main()
        {
            Console.WriteLine(sizeof(void*));
            Console.WriteLine(IntPtr.Size);
            Console.WriteLine(UnsafeUtilities.PointerSize);
            //BenchmarkRunner.Run<SimpleExternalCallBenchmark>();
            //BenchmarkRunner.Run<LoadLargeScriptBenchmark>();
            // var benchmark = new LoadLargeScriptBenchmark();
            // benchmark.Initialize();
            // benchmark.LuaBenchmark();
            // benchmark.DamnScriptBenchmark();
            // benchmark.CompiledScriptParserBenchmark();
        }
    }
}