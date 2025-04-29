# DamnScript Benchmarks Compared to Other Technologies

## Lua Benchmarks

For Lua, the popular framework MoonSharp is used, which allows Lua to be used in .NET applications.
These tests are not a criticism of Lua!
The languages are used for different purposes and have different features, but both are common in games, so the performance difference can be important.

### Calling a Native C# Method from a Script

[Code](https://github.com/Rietmon/DamnScript/blob/main/DamnScriptBenchmarks/SimpleExternalCallBenchmark.cs).

| Method              | Mean        | Error       | StdDev      | Median      | Q1          | Q3          | Gen0     | Gen1     | Gen2     | Allocated   |
|---------------------|-------------|-------------|-------------|-------------|-------------|-------------|----------|----------|----------|-------------|
| LuaBenchmark        | 861.336 µs  | 17.0432 µs  | 20.9306 µs  | 858.024 µs  | 851.325 µs  | 871.733 µs  | 455.0781 | 430.6641 | 416.0156 | 2407.36 KB  |
| DamnScriptBenchmark |   8.921 µs  |  0.1516 µs  |  0.2575 µs  |   8.778 µs  |   8.731 µs  |   9.075 µs  |   1.9684 |   0.0153 |     -    |   12.08 KB  |

### Loading a Large Script into Memory Without Execution

[Code](https://github.com/Rietmon/DamnScript/blob/main/DamnScriptBenchmarks/LoadLargeScriptBenchmark.cs).

This compares Lua, DamnScript, and the compiled version of DamnScript.

| Method                        | Mean      | Error     | StdDev    | Median    | Q1        | Q3        | Gen0     | Gen1     | Gen2     | Allocated  |
|-------------------------------|-----------|-----------|-----------|-----------|-----------|-----------|----------|----------|----------|------------|
| LuaBenchmark                  | 794.20 µs | 19.741 µs | 57.897 µs | 782.47 µs | 754.02 µs | 832.81 µs | 414.0625 | 388.6719 | 361.3281 | 2498.43 KB |
| DamnScriptBenchmark           | 235.06 µs |  1.115 µs |  0.931 µs | 235.12 µs | 234.16 µs | 235.52 µs |  39.5508 |  11.2305 |    -     |  244.68 KB |
| CompiledScriptParserBenchmark |  13.75 µs |  0.266 µs |  0.306 µs |  13.59 µs |  13.51 µs |  14.01 µs |   0.6866 |    -     |    -     |    4.23 KB |

## Pin Benchmark

A custom implementation is used for pinning objects in memory due to limitations of the standard one.
Since it doesn’t work directly with the GC but uses its own methods, it’s slower,
but it eliminates all the restrictions of the standard implementation.

[Code](https://github.com/Rietmon/DamnScript/blob/main/DamnScriptBenchmarks/PinBenchmark.cs).

| Method        | Mean     | Error     | StdDev    | Median   | Q1       | Q3       | Gen0   | Gen1   | Allocated |
|---------------|----------|-----------|-----------|----------|----------|----------|--------|--------|-----------|
| DamnScriptPin | 2.214 µs | 0.0064 µs | 0.0060 µs | 2.217 µs | 2.209 µs | 2.220 µs | 0.6523 | 0.0038 |     4 KB  |
| NETPin        | 1.770 µs | 0.0022 µs | 0.0020 µs | 1.770 µs | 1.769 µs | 1.771 µs | 0.6523 |   -    |     4 KB  |