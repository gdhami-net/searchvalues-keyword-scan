// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285HX 2.80GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-YFEFPZ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

IterationCount=10  WarmupCount=3  

| Method         | Keywords | TextLength | Mean         | Error        | StdDev     | Ratio | RatioSD |
|--------------- |--------- |----------- |-------------:|-------------:|-----------:|------:|--------:|
| ContainsLoop   | 8        | 1000       |    230.91 ns |     6.092 ns |   4.030 ns |  1.00 |    0.02 |
| SearchValues   | 8        | 1000       |     42.73 ns |     0.046 ns |   0.030 ns |  0.19 |    0.00 |
| GeneratedRegex | 8        | 1000       |  1,940.62 ns |     1.235 ns |   0.817 ns |  8.41 |    0.14 |
|                |          |            |              |              |            |       |         |
| ContainsLoop   | 8        | 20000      |  4,186.54 ns |   117.795 ns |  77.914 ns |  1.00 |    0.03 |
| SearchValues   | 8        | 20000      |    845.98 ns |     0.955 ns |   0.632 ns |  0.20 |    0.00 |
| GeneratedRegex | 8        | 20000      | 38,576.19 ns |    26.553 ns |  15.801 ns |  9.22 |    0.16 |
|                |          |            |              |              |            |       |         |
| ContainsLoop   | 40       | 1000       |  1,185.33 ns |     3.400 ns |   1.778 ns |  1.00 |    0.00 |
| SearchValues   | 40       | 1000       |  1,397.34 ns |    34.768 ns |  22.997 ns |  1.18 |    0.02 |
| GeneratedRegex | 40       | 1000       |  4,172.16 ns |     5.883 ns |   3.501 ns |  3.52 |    0.01 |
|                |          |            |              |              |            |       |         |
| ContainsLoop   | 40       | 20000      | 21,787.35 ns |   248.867 ns | 164.610 ns |  1.00 |    0.01 |
| SearchValues   | 40       | 20000      | 26,788.34 ns | 1,388.863 ns | 918.647 ns |  1.23 |    0.04 |
| GeneratedRegex | 40       | 20000      | 90,613.00 ns |   388.961 ns | 231.465 ns |  4.16 |    0.03 |
