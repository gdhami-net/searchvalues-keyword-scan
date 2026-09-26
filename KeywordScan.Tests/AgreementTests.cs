using System.Text.RegularExpressions;
using KeywordScan;
using Xunit;

namespace KeywordScan.Tests;

/// <summary>
/// The benchmark is only worth reading if the three scanners answer the same
/// question. These tests hold them to the same answers on the same inputs.
/// </summary>
public class AgreementTests
{
    public static TheoryData<int, StringComparison, int, double?> Cases()
    {
        var data = new TheoryData<int, StringComparison, int, double?>();
        foreach (var keywordCount in new[] { 8, 40 })
        foreach (var comparison in new[] { StringComparison.Ordinal, StringComparison.OrdinalIgnoreCase })
        foreach (var length in new[] { 1_000, 20_000 })
        foreach (var position in new double?[] { null, 0.0, 0.02, 0.5, 0.98, 1.0 })
            data.Add(keywordCount, comparison, length, position);
        return data;
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void All_three_scanners_agree(int keywordCount, StringComparison comparison, int length, double? position)
    {
        var keywords = Keywords.Take(keywordCount);
        // A keyword that is in the 8-keyword prefix, so every case can hit.
        var text = position is null ? SampleText.Miss(length) : SampleText.Hit(length, "credential", position.Value);

        var loop = new ContainsLoopScanner(keywords, comparison);
        var searchValues = new SearchValuesScanner(keywords, comparison);
        var regex = RegexScanner.For(keywordCount, comparison);

        var loopFound = loop.ContainsAny(text);
        Assert.Equal(loopFound, searchValues.ContainsAny(text));
        Assert.Equal(loopFound, regex.IsMatch(text));
        Assert.Equal(position is not null, loopFound);

        var loopMatch = loop.FirstMatch(text);
        var svMatch = searchValues.FirstMatch(text);
        Assert.Equal(loopMatch, svMatch);
        Assert.Equal(loopMatch.Index, searchValues.IndexOfAny(text));

        var regexMatch = regex.Match(text);
        Assert.Equal(loopMatch.Index, regexMatch.Success ? regexMatch.Index : -1);
        if (regexMatch.Success)
        {
            Assert.Equal(loopMatch.Keyword, regexMatch.Value.ToLowerInvariant());
        }
    }

    [Theory]
    [InlineData(8)]
    [InlineData(40)]
    public void Mixed_case_text_matches_only_under_ignore_case(int keywordCount)
    {
        var keywords = Keywords.Take(keywordCount);
        var text = SampleText.Miss(2_000)[..1_000] + "CrEdEnTiAl" + SampleText.Miss(2_000)[1_010..];

        Assert.False(new SearchValuesScanner(keywords, StringComparison.Ordinal).ContainsAny(text));
        Assert.True(new SearchValuesScanner(keywords, StringComparison.OrdinalIgnoreCase).ContainsAny(text));
        Assert.Equal(1_000, new SearchValuesScanner(keywords, StringComparison.OrdinalIgnoreCase).IndexOfAny(text));

        Assert.DoesNotMatch(RegexScanner.For(keywordCount, StringComparison.Ordinal), text);
        Assert.Matches(RegexScanner.For(keywordCount, StringComparison.OrdinalIgnoreCase), text);
    }

    [Theory]
    [InlineData(1_000)]
    [InlineData(20_000)]
    public void Filler_text_contains_none_of_the_keywords(int length)
    {
        var text = SampleText.Miss(length);
        Assert.Equal(length, text.Length);
        foreach (var keyword in Keywords.All)
        {
            Assert.DoesNotContain(keyword, text, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Hit_text_keeps_the_length_of_the_miss_text()
    {
        Assert.Equal(20_000, SampleText.Hit(20_000, "credential", 0.5).Length);
    }

    /// <summary>
    /// SearchValuesScanner.FirstMatch assumes at most one keyword can start at
    /// any given position. That holds because no keyword is a prefix of
    /// another; if the list ever gains such a pair the assumption breaks.
    /// </summary>
    [Fact]
    public void No_keyword_is_a_prefix_of_another()
    {
        foreach (var a in Keywords.All)
        foreach (var b in Keywords.All)
        {
            if (!ReferenceEquals(a, b))
            {
                Assert.False(b.StartsWith(a, StringComparison.OrdinalIgnoreCase),
                    $"'{a}' is a prefix of '{b}'");
            }
        }
    }

    /// <summary>
    /// The list order decides which keyword a naive loop reports; the text
    /// decides which one SearchValues finds. ContainsLoopScanner.FirstMatch
    /// takes the minimum index for that reason.
    /// </summary>
    [Fact]
    public void Earliest_match_in_the_text_is_not_the_first_keyword_in_the_list()
    {
        var keywords = Keywords.Take(8);   // "password" is [0], "credential" is [6]
        var text = "a credential and then a password";

        var firstInList = keywords.First(text.Contains);
        Assert.Equal("password", firstInList);

        Assert.Equal((2, "credential"), new SearchValuesScanner(keywords, StringComparison.Ordinal).FirstMatch(text));
        Assert.Equal((2, "credential"), new ContainsLoopScanner(keywords, StringComparison.Ordinal).FirstMatch(text));
    }
}
