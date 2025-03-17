using System.Collections.Generic;
using System.Diagnostics;

namespace DamnScript.Runtimes.Debugs
{
	public static class Profiler
	{
		public static readonly Dictionary<string, SampleData> samples = new();
		
		public static void Clear()
		{
			samples.Clear();
		}
		
		public static void BeginSample(string name)
		{
			if (!samples.TryGetValue(name, out var sample))
			{
				samples.Add(name, new SampleData { name = name, ticks = 0, count = 1, startTicks = Stopwatch.GetTimestamp() });
			}
			else
			{
				sample.count++;
				sample.startTicks = Stopwatch.GetTimestamp();
				samples[name] = sample;
			}
		}
		
		public static void EndSample(string name)
		{
			if (samples.TryGetValue(name, out var sample))
			{
				sample.ticks += Stopwatch.GetTimestamp() - sample.startTicks;
				samples[name] = sample;
			}
			else
				throw new System.Exception("Sample not found: " + name);
		}
		
		public static string GetReport()
		{
			var report = "";
			foreach (var sample in samples.Values)
			{
				report += $"{sample.name}: {sample.Milliseconds} ms, {sample.MedianMilliseconds} M ms, {sample.count} calls\n";
			}
			return report;
		}
	}

	public struct SampleData
	{
		public string name;
		public long ticks;
		public int count;
		
		public long startTicks;
		
		public double Milliseconds => ticks / (double)Stopwatch.Frequency * 1000d;
		public double MedianMilliseconds => ticks / (double)count / Stopwatch.Frequency * 1000d;
	}
}