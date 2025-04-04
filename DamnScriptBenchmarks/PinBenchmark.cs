using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using DamnScript.Runtimes.Cores.Pins;

namespace DamnScriptBenchmarks;

public class TestPin
{
	public decimal value;
	public int value2;
	public int value3;
	public int value4;
	public int value5;
	public int value6;
	public int value7;
	public int value8;
}

[Config(typeof(BenchmarksConfig))]
public class PinBenchmark
{
	public static ObjectPin[] pinsDS = new ObjectPin[64];
	public static GCHandle[] pinsNET = new GCHandle[64];
	
	[Benchmark, MethodImpl(MethodImplOptions.NoOptimization)]
	public void DamnScriptPin()
	{
		for (var i = 0; i < pinsDS.Length; i++)
			pinsDS[i] = PinHelper.Pin(new TestPin());

		for (var i = 0; i < pinsDS.Length; i++)
		{
			var pin = pinsDS[i].Target;
		}
		
		for (var i = 0; i < pinsDS.Length; i++)
			pinsDS[i].Free();
	}
	
	[Benchmark, MethodImpl(MethodImplOptions.NoOptimization)]
	public void NETPin()
	{
		for (var i = 0; i < pinsNET.Length; i++)
			pinsNET[i] = GCHandle.Alloc(new TestPin(), GCHandleType.Pinned);

		for (var i = 0; i < pinsNET.Length; i++)
		{
			var pin = pinsNET[i].Target;
		}
		
		for (var i = 0; i < pinsNET.Length; i++)
			pinsNET[i].Free();
	}
}