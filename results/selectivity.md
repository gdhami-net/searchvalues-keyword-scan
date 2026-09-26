// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285HX 2.80GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-YFEFPZ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

IterationCount=10  WarmupCount=3  

| Method       | Prefixes | Mean        | Error       | StdDev      | Ratio | RatioSD |
|------------- |--------- |------------:|------------:|------------:|------:|--------:|
| ContainsLoop | absent   | 18,295.3 ns |    58.09 ns |    38.42 ns |  1.00 |    0.00 |
| SearchValues | absent   |    843.3 ns |     0.54 ns |     0.36 ns |  0.05 |    0.00 |
|              |          |             |             |             |       |         |
| ContainsLoop | common   | 21,158.2 ns |   392.36 ns |   259.52 ns |  1.00 |    0.02 |
| SearchValues | common   | 27,084.5 ns | 2,171.41 ns | 1,436.25 ns |  1.28 |    0.07 |
