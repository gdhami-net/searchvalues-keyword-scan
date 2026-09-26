using System.Buffers;
using System.Text.RegularExpressions;

namespace KeywordScan;

/// <summary>The loop almost everybody writes first.</summary>
public sealed class ContainsLoopScanner(string[] keywords, StringComparison comparison)
{
    public bool ContainsAny(string text)
    {
        foreach (var keyword in keywords)
        {
            if (text.Contains(keyword, comparison)) return true;
        }
        return false;
    }

    /// <summary>
    /// Earliest match in the TEXT, not the first keyword in the list that
    /// happens to be present. Written out because the naive loop returns the
    /// latter and the two are not the same answer.
    /// </summary>
    public (int Index, string? Keyword) FirstMatch(string text)
    {
        var best = -1;
        string? bestKeyword = null;
        foreach (var keyword in keywords)
        {
            var at = text.IndexOf(keyword, comparison);
            if (at >= 0 && (best < 0 || at < best))
            {
                best = at;
                bestKeyword = keyword;
            }
        }
        return (best, bestKeyword);
    }
}

/// <summary>One pass over the text, whatever the keyword count.</summary>
public sealed class SearchValuesScanner
{
    private readonly SearchValues<string> _values;
    private readonly string[] _keywords;
    private readonly StringComparison _comparison;

    public SearchValuesScanner(string[] keywords, StringComparison comparison)
    {
        // Built once. Building it per call costs more than the scan it replaces.
        _values = SearchValues.Create(keywords, comparison);
        _keywords = keywords;
        _comparison = comparison;
    }

    public bool ContainsAny(string text) => text.AsSpan().ContainsAny(_values);

    public int IndexOfAny(string text) => text.AsSpan().IndexOfAny(_values);

    /// <summary>
    /// IndexOfAny reports where, not which. Getting the keyword back means one
    /// short pass over the list at the position it found. No keyword in
    /// <see cref="Keywords.All"/> is a prefix of another, so exactly one can
    /// start there; with prefix pairs in the list you would keep the longest.
    /// </summary>
    public (int Index, string? Keyword) FirstMatch(string text)
    {
        var at = text.AsSpan().IndexOfAny(_values);
        if (at < 0) return (-1, null);

        foreach (var keyword in _keywords)
        {
            if (text.AsSpan(at).StartsWith(keyword, _comparison)) return (at, keyword);
        }
        throw new InvalidOperationException($"IndexOfAny reported a match at {at} that no keyword claims.");
    }
}

/// <summary>
/// The source-generated alternation. The pattern has to be a compile-time
/// literal, which is why the two keyword counts are two separate methods.
/// </summary>
public static partial class RegexScanner
{
    [GeneratedRegex("password|secret|token|apikey|api_key|private key|credential|passwd",
        RegexOptions.CultureInvariant)]
    public static partial Regex Ordinal8();

    [GeneratedRegex("password|secret|token|apikey|api_key|private key|credential|passwd|authorization|bearer|session|connectionstring|pwd=|client_secret|refresh_token|access_token|ssh-rsa|aws_secret|azure_storage|sas_token|x-api-key|jdbc:|mongodb://|postgres://|redis://|amqp://|smtp://|user id=|integrated security|trustservercertificate|encrypt=false|keystore|truststore|pkcs12|keyvault|vault_token|service_account|privatekey|clientsecret|sharedaccesskey",
        RegexOptions.CultureInvariant)]
    public static partial Regex Ordinal40();

    [GeneratedRegex("password|secret|token|apikey|api_key|private key|credential|passwd",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex IgnoreCase8();

    [GeneratedRegex("password|secret|token|apikey|api_key|private key|credential|passwd|authorization|bearer|session|connectionstring|pwd=|client_secret|refresh_token|access_token|ssh-rsa|aws_secret|azure_storage|sas_token|x-api-key|jdbc:|mongodb://|postgres://|redis://|amqp://|smtp://|user id=|integrated security|trustservercertificate|encrypt=false|keystore|truststore|pkcs12|keyvault|vault_token|service_account|privatekey|clientsecret|sharedaccesskey",
        RegexOptions.CultureInvariant | RegexOptions.IgnoreCase)]
    public static partial Regex IgnoreCase40();

    public static Regex For(int keywordCount, StringComparison comparison) =>
        (keywordCount, comparison) switch
        {
            (8, StringComparison.Ordinal) => Ordinal8(),
            (40, StringComparison.Ordinal) => Ordinal40(),
            (8, StringComparison.OrdinalIgnoreCase) => IgnoreCase8(),
            (40, StringComparison.OrdinalIgnoreCase) => IgnoreCase40(),
            _ => throw new ArgumentOutOfRangeException(nameof(keywordCount),
                $"No generated pattern for {keywordCount} keywords / {comparison}."),
        };
}
