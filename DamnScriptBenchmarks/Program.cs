using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public class test
        {
            public int a;
            public test2 b;
        }

        public class test2
        {
            public string a;
        }
        
        public static void Main()
        {
            // var stopWatch = new System.Diagnostics.Stopwatch();
            // stopWatch.Start();
            // {
            //     var handles = new ObjectPin[10000];
            //     for (var i = 0; i < handles.Length; i++)
            //     {
            //         var pin = PinHelper.Pin(new TestPin());
            //         handles[i] = pin;
            //     }
            // }
            // stopWatch.Stop();
            // Console.WriteLine($"DamnScript Pin: {stopWatch.ElapsedMilliseconds} ms");
            // stopWatch.Reset();
            // stopWatch.Start();
            // {
            //     var handles = new GCHandle[10000];
            //     for (var i = 0; i < handles.Length; i++)
            //     {
            //         var pin = GCHandle.Alloc(new TestPin(), GCHandleType.Pinned);
            //         handles[i] = pin;
            //     }
            // }
            // stopWatch.Stop();
            // Console.WriteLine($"NET Pin: {stopWatch.ElapsedMilliseconds} ms");
            BenchmarkRunner.Run<PinBenchmark>();
            // BenchmarkRunner.Run<LoadLargeScriptBenchmark>();
            // var benchmark = new LoadLargeScriptBenchmark();
            // benchmark.Initialize();
            // benchmark.LuaBenchmark();
            // benchmark.DamnScriptBenchmark();
            // benchmark.CompiledScriptParserBenchmark();
        }
    }
}