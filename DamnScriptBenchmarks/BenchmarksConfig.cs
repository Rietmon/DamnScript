using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;

namespace DamnScriptBenchmarks
{
	public class BenchmarksConfig : ManualConfig
	{
		public BenchmarksConfig()
		{
			AddDiagnoser(MemoryDiagnoser.Default);
			AddLogger(ConsoleLogger.Default);
			AddColumn(TargetMethodColumn.Method, StatisticColumn.Median, StatisticColumn.StdDev,
				StatisticColumn.Q1, StatisticColumn.Q3, new ParamColumn("Size"));
		}
	}
}