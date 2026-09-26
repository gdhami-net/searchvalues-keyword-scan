// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285HX 2.80GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-YFEFPZ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

IterationCount=10  WarmupCount=3  

| Method                         | Mean        | Error       | StdDev    | Ratio | RatioSD | Gen0   | Gen1   | Allocated | Alloc Ratio |
|------------------------------- |------------:|------------:|----------:|------:|--------:|-------:|-------:|----------:|------------:|
| Create_8_keywords              |    627.7 ns |     6.07 ns |   3.61 ns |  0.03 |    0.00 | 0.1440 |      - |    2712 B |          NA |
| Create_40_keywords             |  4,186.8 ns |    66.96 ns |  44.29 ns |  0.20 |    0.00 | 0.5341 | 0.0076 |   10144 B |          NA |
| New_Regex_Compiled_40_keywords | 38,555.3 ns | 1,301.05 ns | 680.48 ns |  1.85 |    0.03 | 7.3242 | 0.7324 |  140288 B |          NA |
| Create_then_scan_20k           | 29,473.4 ns |   430.02 ns | 284.43 ns |  1.42 |    0.02 | 0.5188 |      - |   10144 B |          NA |
| ContainsLoop_20k               | 20,786.0 ns |   272.18 ns | 180.03 ns |  1.00 |    0.01 |      - |      - |         - |          NA |
