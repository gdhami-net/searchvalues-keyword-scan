# searchvalues-keyword-scan

Companion repo for **"How much faster SearchValues&lt;string&gt; finds keywords
than a Contains loop"** ([gdhami.net](https://gdhami.net) — link added when the
post is live).

Three ways to ask "does this text contain any of my keywords", measured against
each other with BenchmarkDotNet on .NET 10, plus an xUnit suite that holds all
three to the same answers:

- a loop of `text.Contains(keyword, comparison)`
- `SearchValues<string>` built once, then `text.AsSpan().ContainsAny(values)`
- a `[GeneratedRegex]` alternation of the same keywords

## What it proves

The short version: one pass is not automatically cheaper than forty passes.
`string.Contains` under `StringComparison.Ordinal` is vectorised, so a loop of
forty of them can beat one `SearchValues` pass — and does, on this text. What
decides it is how often the SIMD prefilter inside `SearchValues` has to stop and
check a candidate, which depends on the keywords' first characters and on the
text, not on the keyword count.

| Check | Claim |
|---|---|
| `AgreementTests.All_three_scanners_agree` | 48 combinations of keyword count, comparison, text length and match position: the same yes/no answer, the same earliest-match index, the same matched keyword |
| `AgreementTests.Filler_text_contains_none_of_the_keywords` | the "miss" text really is a miss, under `OrdinalIgnoreCase`, so the benchmark is not measuring an early exit |
| `AgreementTests.Earliest_match_in_the_text_is_not_the_first_keyword_in_the_list` | a naive `Contains` loop reports whichever keyword is first in the LIST; `IndexOfAny` finds whichever is first in the TEXT |
| `AgreementTests.No_keyword_is_a_prefix_of_another` | the assumption that makes "which keyword matched" recoverable from one index |
| `SearchValuesApiTests.Create_rejects_every_comparison_but_the_two_ordinal_ones` | the exact `ArgumentException` message and its `ParamName` |
| `SearchValuesApiTests.An_empty_keyword_makes_everything_a_match` | an empty string in the list is accepted and matches at position 0 |
| `SearchValuesApiTests.Kelvin_sign_separates_OrdinalIgnoreCase_from_RegexOptions_IgnoreCase` | U+212A matches `apikey` for `Regex` with `IgnoreCase` and does not for `OrdinalIgnoreCase` |
| `SearchValuesApiTests.Create_picks_a_different_implementation_for_eight_keywords_and_forty` | `Create` chooses a strategy from the keyword set (internal type names, recorded so a runtime change fails loudly) |

## Run it

```bash
dotnet test --configuration Release     # or ./check.sh / ./check.ps1
```

The benchmarks, one class at a time (each takes a few minutes, and the whole set
takes longer than you want to sit through):

```bash
dotnet run -c Release --project KeywordScan.Benchmarks -- --filter '*MissOrdinalBenchmarks*'
dotnet run -c Release --project KeywordScan.Benchmarks -- --filter '*MissIgnoreCaseBenchmarks*'
dotnet run -c Release --project KeywordScan.Benchmarks -- --filter '*SelectivityBenchmarks*'
dotnet run -c Release --project KeywordScan.Benchmarks -- --filter '*CrossoverBenchmarks*'
dotnet run -c Release --project KeywordScan.Benchmarks -- --filter '*HitBenchmarks*'
dotnet run -c Release --project KeywordScan.Benchmarks -- --filter '*BuildBenchmarks*'
```

`results/` holds the summary tables quoted in the post, exactly as
BenchmarkDotNet printed them, machine header included:

| File | What it measures |
|---|---|
| `miss-ordinal.md` | 8 and 40 keywords over 1 000 and 20 000 characters, `Ordinal`, no match |
| `miss-ignorecase.md` | the same grid under `OrdinalIgnoreCase` |
| `crossover.md` | 8, 16, 24, 32 and 40 keywords at 20 000 characters |
| `selectivity.md` | 40 keywords whose prefixes are common in the text, and the same 40 with prefixes absent from it |
| `hit-ordinal.md` | one keyword present at 2% and at 98% of the text |
| `build-cost.md` | `SearchValues.Create`, `new Regex(Compiled)`, and building per call |

Your absolute numbers will differ; the ratios are the part worth comparing. On
this machine the 40-keyword `SearchValues` mean moved between 26.36 and 27.08 µs
across three runs of the same build.

## What is where

| Path | Contents |
|---|---|
| `KeywordScan/Keywords.cs` | the forty keywords, and the same forty with prefixes that never occur in the sample text |
| `KeywordScan/SampleText.cs` | deterministic filler: a fixed sequence over a fixed vocabulary, so a test and a benchmark see identical input |
| `KeywordScan/Scanners.cs` | the three scanners, plus the follow-up pass that recovers which keyword matched |
| `KeywordScan.Tests/` | the agreement suite and the API-edge suite |
| `KeywordScan.Benchmarks/` | the BenchmarkDotNet classes |

The regex source generator's output is written to
`KeywordScan/obj/Release/net10.0/generated/` (`EmitCompilerGeneratedFiles` is on
in the csproj), which is where the `SearchValues` prefilters that the generated
matcher picks for itself can be read.

Measured on .NET SDK 10.0.201, runtime 10.0.5, `net10.0`, x64 with AVX2,
BenchmarkDotNet 0.15.8, Release, no debugger attached. `SearchValues<string>`
needs .NET 9 or later; the rest of the code compiles on .NET 9 too.

MIT licensed.
