// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285HX 2.80GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-YFEFPZ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

IterationCount=10  WarmupCount=3  

| Method       | Keywords | Mean        | Error     | StdDev    | Ratio | RatioSD |
|------------- |--------- |------------:|----------:|----------:|------:|--------:|
| ContainsLoop | 8        |  4,065.0 ns |  48.18 ns |  31.87 ns |  1.00 |    0.01 |
| SearchValues | 8        |    846.0 ns |   0.34 ns |   0.18 ns |  0.21 |    0.00 |
|              |          |             |           |           |       |         |
| ContainsLoop | 16       |  8,317.1 ns | 113.12 ns |  74.82 ns |  1.00 |    0.01 |
| SearchValues | 16       |  1,620.0 ns |   2.62 ns |   1.73 ns |  0.19 |    0.00 |
|              |          |             |           |           |       |         |
| ContainsLoop | 24       | 12,323.6 ns |  88.26 ns |  52.52 ns |  1.00 |    0.01 |
| SearchValues | 24       |  5,084.9 ns |  11.29 ns |   7.47 ns |  0.41 |    0.00 |
|              |          |             |           |           |       |         |
| ContainsLoop | 32       | 16,451.3 ns | 315.06 ns | 208.39 ns |  1.00 |    0.02 |
| SearchValues | 32       | 15,931.3 ns | 859.61 ns | 568.58 ns |  0.97 |    0.03 |
|              |          |             |           |           |       |         |
| ContainsLoop | 40       | 21,192.1 ns | 376.45 ns | 249.00 ns |  1.00 |    0.02 |
| SearchValues | 40       | 26,359.9 ns | 888.34 ns | 587.58 ns |  1.24 |    0.03 |
