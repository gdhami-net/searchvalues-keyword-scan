// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285HX 2.80GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-YFEFPZ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

IterationCount=10  WarmupCount=3  

| Method         | Keywords | TextLength | Mean          | Error        | StdDev       | Ratio | RatioSD |
|--------------- |--------- |----------- |--------------:|-------------:|-------------:|------:|--------:|
| ContainsLoop   | 8        | 1000       |     260.43 ns |     6.669 ns |     4.411 ns |  1.00 |    0.02 |
| SearchValues   | 8        | 1000       |      43.49 ns |     0.038 ns |     0.023 ns |  0.17 |    0.00 |
| GeneratedRegex | 8        | 1000       |      78.34 ns |     0.215 ns |     0.142 ns |  0.30 |    0.00 |
|                |          |            |               |              |              |       |         |
| ContainsLoop   | 8        | 20000      |   4,745.77 ns |    31.856 ns |    21.071 ns |  1.00 |    0.01 |
| SearchValues   | 8        | 20000      |     861.10 ns |     0.504 ns |     0.264 ns |  0.18 |    0.00 |
| GeneratedRegex | 8        | 20000      |   1,860.70 ns |     1.841 ns |     1.218 ns |  0.39 |    0.00 |
|                |          |            |               |              |              |       |         |
| ContainsLoop   | 40       | 1000       |   1,517.07 ns |    19.179 ns |    11.413 ns |  1.00 |    0.01 |
| SearchValues   | 40       | 1000       |   1,901.65 ns |    57.452 ns |    38.001 ns |  1.25 |    0.03 |
| GeneratedRegex | 40       | 1000       |   6,799.50 ns |     2.009 ns |     1.329 ns |  4.48 |    0.03 |
|                |          |            |               |              |              |       |         |
| ContainsLoop   | 40       | 20000      |  28,147.99 ns |   446.893 ns |   295.592 ns |  1.00 |    0.01 |
| SearchValues   | 40       | 20000      |  36,350.19 ns | 1,567.471 ns | 1,036.785 ns |  1.29 |    0.04 |
| GeneratedRegex | 40       | 20000      | 134,556.45 ns | 7,074.687 ns | 4,679.467 ns |  4.78 |    0.17 |
