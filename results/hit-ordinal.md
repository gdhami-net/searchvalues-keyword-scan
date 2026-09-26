// * Summary *

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26200.9457/25H2/2025Update/HudsonValley2)
Intel Core Ultra 9 285HX 2.80GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.201
  [Host]     : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3
  Job-YFEFPZ : .NET 10.0.5 (10.0.5, 10.0.526.15411), X64 RyuJIT x86-64-v3

IterationCount=10  WarmupCount=3  

| Method         | Position | Mean        | Error       | StdDev      | Ratio | RatioSD |
|--------------- |--------- |------------:|------------:|------------:|------:|--------:|
| ContainsLoop   | 0.02     | 20,425.7 ns |   152.58 ns |   100.93 ns |  1.00 |    0.01 |
| SearchValues   | 0.02     |    599.5 ns |     8.38 ns |     5.54 ns |  0.03 |    0.00 |
| GeneratedRegex | 0.02     |  1,719.2 ns |     1.34 ns |     0.89 ns |  0.08 |    0.00 |
|                |          |             |             |             |       |         |
| ContainsLoop   | 0.98     | 20,797.6 ns |   266.84 ns |   176.50 ns |  1.00 |    0.01 |
| SearchValues   | 0.98     | 25,125.7 ns | 1,244.28 ns |   823.01 ns |  1.21 |    0.04 |
| GeneratedRegex | 0.98     | 88,899.3 ns | 1,667.79 ns | 1,103.14 ns |  4.27 |    0.06 |
