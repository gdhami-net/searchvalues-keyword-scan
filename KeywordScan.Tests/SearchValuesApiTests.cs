using System.Buffers;
using System.Text.RegularExpressions;
using KeywordScan;
using Xunit;

namespace KeywordScan.Tests;

/// <summary>
/// The edges of SearchValues&lt;string&gt; itself: what Create refuses, what it
/// quietly accepts, and where OrdinalIgnoreCase and RegexOptions.IgnoreCase
/// stop agreeing.
/// </summary>
public class SearchValuesApiTests
{
    [Theory]
    [InlineData(StringComparison.CurrentCulture)]
    [InlineData(StringComparison.CurrentCultureIgnoreCase)]
    [InlineData(StringComparison.InvariantCulture)]
    [InlineData(StringComparison.InvariantCultureIgnoreCase)]
    public void Create_rejects_every_comparison_but_the_two_ordinal_ones(StringComparison comparison)
    {
        var ex = Assert.Throws<ArgumentException>(
            () => SearchValues.Create(Keywords.All, comparison));

        Assert.Equal("comparisonType", ex.ParamName);
        Assert.StartsWith(
            "SearchValues<string> supports only StringComparison.Ordinal and StringComparison.OrdinalIgnoreCase.",
            ex.Message);
    }

    [Theory]
    [InlineData(StringComparison.Ordinal)]
    [InlineData(StringComparison.OrdinalIgnoreCase)]
    public void Create_accepts_the_two_ordinal_comparisons(StringComparison comparison)
    {
        Assert.NotNull(SearchValues.Create(Keywords.All, comparison));
    }

    /// <summary>
    /// An empty string in the list is not rejected, and it matches at position
    /// zero of anything. Easy to produce from a config file with a blank line.
    /// </summary>
    [Fact]
    public void An_empty_keyword_makes_everything_a_match()
    {
        var values = SearchValues.Create(["password", ""], StringComparison.Ordinal);

        Assert.Equal(0, "nothing to see here".AsSpan().IndexOfAny(values));
        Assert.True("nothing to see here".AsSpan().ContainsAny(values));
    }

    /// <summary>
    /// IndexOfAny answers where, not which. This is the follow-up pass that
    /// gets the keyword back, and it costs one short walk of the list at one
    /// position instead of a walk of the whole text.
    /// </summary>
    [Fact]
    public void Recovering_the_matched_keyword_needs_a_second_look_at_the_hit()
    {
        var scanner = new SearchValuesScanner(Keywords.All, StringComparison.OrdinalIgnoreCase);
        var text = SampleText.Hit(5_000, "refresh_token", 0.4);

        var index = scanner.IndexOfAny(text);
        Assert.True(index > 0);
        Assert.Equal((index, "refresh_token"), scanner.FirstMatch(text));
    }

    /// <summary>
    /// OrdinalIgnoreCase and RegexOptions.IgnoreCase are not the same relation.
    /// U+212A KELVIN SIGN is case-equivalent to 'k' for Regex and is not for
    /// OrdinalIgnoreCase, so the two disagree on this input. Verified on
    /// .NET 10.0.5; the Regex side of it changed in .NET 7.
    /// </summary>
    [Fact]
    public void Kelvin_sign_separates_OrdinalIgnoreCase_from_RegexOptions_IgnoreCase()
    {
        const char kelvinSign = (char)0x212A;   // U+212A KELVIN SIGN
        var text = "the token is api" + kelvinSign + "ey now";

        var searchValues = SearchValues.Create(["apikey"], StringComparison.OrdinalIgnoreCase);
        Assert.False(text.AsSpan().ContainsAny(searchValues));
        Assert.False(text.Contains("apikey", StringComparison.OrdinalIgnoreCase));

        Assert.Matches(new Regex("apikey", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant), text);
    }

    /// <summary>
    /// Create picks a strategy from the keyword set, so the concrete type is
    /// not the same for eight keywords and forty. These names are internal and
    /// can change with the runtime; the test exists to show the choice is being
    /// made, and to fail loudly if a future runtime stops making it.
    /// </summary>
    [Fact]
    public void Create_picks_a_different_implementation_for_eight_keywords_and_forty()
    {
        var eight = SearchValues.Create(Keywords.Take(8), StringComparison.Ordinal).GetType().Name;
        var forty = SearchValues.Create(Keywords.All, StringComparison.Ordinal).GetType().Name;

        Assert.Equal("AsciiStringSearchValuesTeddyNonBucketizedN3`2", eight);
        Assert.Equal("AsciiStringSearchValuesTeddyBucketizedN3`2", forty);
        Assert.NotEqual(eight, forty);
    }
}
