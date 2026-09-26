using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using KeywordScan;

namespace KeywordScan.Benchmarks;

/// <summary>The three scanners, asking the same yes/no question of the same text.</summary>
public abstract class ScanBenchmarkBase
{
    protected string Text = "";
    protected ContainsLoopScanner Loop = null!;
    protected SearchValuesScanner Values = null!;
    protected Regex Pattern = null!;

    [Benchmark(Baseline = true)]
    public bool ContainsLoop() => Loop.ContainsAny(Text);

    [Benchmark]
    public bool SearchValues() => Values.ContainsAny(Text);

    [Benchmark]
    public bool GeneratedRegex() => Pattern.IsMatch(Text);
}

/// <summary>
/// No keyword is present, ordinal. This is the case a filter spends nearly all
/// its time on, and the one where nothing can stop early.
/// </summary>
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class MissOrdinalBenchmarks : ScanBenchmarkBase
{
    [Params(8, 40)]
    public int Keywords { get; set; }

    [Params(1_000, 20_000)]
    public int TextLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var keywords = KeywordScan.Keywords.Take(Keywords);
        Text = SampleText.Miss(TextLength);
        Loop = new ContainsLoopScanner(keywords, StringComparison.Ordinal);
        Values = new SearchValuesScanner(keywords, StringComparison.Ordinal);
        Pattern = RegexScanner.For(Keywords, StringComparison.Ordinal);
    }
}

/// <summary>The same grid under OrdinalIgnoreCase / RegexOptions.IgnoreCase.</summary>
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class MissIgnoreCaseBenchmarks : ScanBenchmarkBase
{
    [Params(8, 40)]
    public int Keywords { get; set; }

    [Params(1_000, 20_000)]
    public int TextLength { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var keywords = KeywordScan.Keywords.Take(Keywords);
        Text = SampleText.Miss(TextLength);
        Loop = new ContainsLoopScanner(keywords, StringComparison.OrdinalIgnoreCase);
        Values = new SearchValuesScanner(keywords, StringComparison.OrdinalIgnoreCase);
        Pattern = RegexScanner.For(Keywords, StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// A keyword IS present, near the front and near the back of the same 20 000
/// characters, ordinal, 40 keywords. The present keyword is the last of the
/// forty, so the loop tries the other 39 before it finds anything.
/// </summary>
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class HitBenchmarks : ScanBenchmarkBase
{
    [Params(0.02, 0.98)]
    public double Position { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var keywords = KeywordScan.Keywords.All;
        Text = SampleText.Hit(20_000, "sharedaccesskey", Position);
        Loop = new ContainsLoopScanner(keywords, StringComparison.Ordinal);
        Values = new SearchValuesScanner(keywords, StringComparison.Ordinal);
        Pattern = RegexScanner.For(40, StringComparison.Ordinal);
    }
}

/// <summary>
/// Forty keywords either way; the only thing that changes is whether their
/// first characters occur in the text. No Regex column here because a
/// [GeneratedRegex] pattern has to be a compile-time literal.
/// </summary>
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class SelectivityBenchmarks
{
    /// <summary>"common" = the forty keywords as they are; "absent" = the same
    /// forty with a three-character prefix the text never contains.</summary>
    [Params("common", "absent")]
    public string Prefixes { get; set; } = "";

    private string _text = "";
    private ContainsLoopScanner _loop = null!;
    private SearchValuesScanner _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var keywords = Prefixes == "absent"
            ? Keywords.AllWithRarePrefixes
            : Keywords.All;
        _text = SampleText.Miss(20_000);
        _loop = new ContainsLoopScanner(keywords, StringComparison.Ordinal);
        _values = new SearchValuesScanner(keywords, StringComparison.Ordinal);
    }

    [Benchmark(Baseline = true)]
    public bool ContainsLoop() => _loop.ContainsAny(_text);

    [Benchmark]
    public bool SearchValues() => _values.ContainsAny(_text);
}

/// <summary>
/// Where the two cross over on this text, in steps of eight keywords. No Regex
/// column: a [GeneratedRegex] pattern has to be a compile-time literal, and
/// only the 8- and 40-keyword patterns exist.
/// </summary>
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CrossoverBenchmarks
{
    [Params(8, 16, 24, 32, 40)]
    public int Keywords { get; set; }

    private string _text = "";
    private ContainsLoopScanner _loop = null!;
    private SearchValuesScanner _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var keywords = KeywordScan.Keywords.Take(Keywords);
        _text = SampleText.Miss(20_000);
        _loop = new ContainsLoopScanner(keywords, StringComparison.Ordinal);
        _values = new SearchValuesScanner(keywords, StringComparison.Ordinal);
    }

    [Benchmark(Baseline = true)]
    public bool ContainsLoop() => _loop.ContainsAny(_text);

    [Benchmark]
    public bool SearchValues() => _values.ContainsAny(_text);
}

/// <summary>
/// What it costs to build the thing, and what happens if you build it per call
/// instead of caching it in a static field.
/// </summary>
[SimpleJob(warmupCount: 3, iterationCount: 10)]
[MemoryDiagnoser]
public class BuildBenchmarks
{
    private readonly string[] _eight = KeywordScan.Keywords.Take(8);
    private readonly string[] _forty = KeywordScan.Keywords.All;
    private readonly string _pattern = string.Join('|', KeywordScan.Keywords.All);
    private readonly string _text = SampleText.Miss(20_000);
    private ContainsLoopScanner _loop = null!;

    [GlobalSetup]
    public void Setup() => _loop = new ContainsLoopScanner(_forty, StringComparison.Ordinal);

    [Benchmark]
    public object Create_8_keywords() => System.Buffers.SearchValues.Create(_eight, StringComparison.Ordinal);

    [Benchmark]
    public object Create_40_keywords() => System.Buffers.SearchValues.Create(_forty, StringComparison.Ordinal);

    [Benchmark]
    public object New_Regex_Compiled_40_keywords() =>
        new Regex(_pattern, RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Benchmark]
    public bool Create_then_scan_20k() =>
        _text.AsSpan().ContainsAny(System.Buffers.SearchValues.Create(_forty, StringComparison.Ordinal));

    [Benchmark(Baseline = true)]
    public bool ContainsLoop_20k() => _loop.ContainsAny(_text);
}
