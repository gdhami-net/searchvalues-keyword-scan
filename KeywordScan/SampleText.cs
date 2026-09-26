using System.Text;

namespace KeywordScan;

/// <summary>
/// Deterministic filler text. Same length in, same characters out, on every
/// machine and every run: the generator is a fixed linear congruential
/// sequence over a fixed vocabulary, so a benchmark and a test see byte-equal
/// input.
/// </summary>
public static class SampleText
{
    private static readonly string[] Vocabulary =
    [
        "the", "build", "agent", "queued", "three", "jobs", "before", "lunch",
        "runner", "picked", "them", "up", "again", "after", "cache", "warmed",
        "retry", "window", "closed", "at", "noon", "and", "nothing", "in",
        "queue", "moved", "until", "worker", "restarted", "itself", "twice",
        "logs", "show", "request", "finished", "with", "status", "two",
        "hundred", "on", "first", "attempt", "no", "error", "was", "written",
        "to", "sink", "so", "batch", "went", "through", "clean",
    ];

    /// <summary>
    /// Filler of exactly <paramref name="length"/> characters that contains
    /// none of <see cref="Keywords.All"/>. The tests assert that emptiness
    /// instead of trusting it.
    /// </summary>
    public static string Miss(int length)
    {
        var sb = new StringBuilder(length + 16);
        uint state = 20260901u;
        while (sb.Length < length)
        {
            state = state * 1664525u + 1013904223u;
            if (sb.Length > 0) sb.Append(' ');
            sb.Append(Vocabulary[(int)(state >> 16) % Vocabulary.Length]);
        }
        return sb.ToString(0, length);
    }

    /// <summary>
    /// The same filler with <paramref name="keyword"/> overwritten in at
    /// <paramref name="fraction"/> of the way through, so the length is
    /// identical to <see cref="Miss"/> for the same <paramref name="length"/>.
    /// </summary>
    public static string Hit(int length, string keyword, double fraction)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(length, keyword.Length);
        var text = Miss(length).ToCharArray();
        var start = (int)(fraction * (length - keyword.Length));
        keyword.CopyTo(text.AsSpan(start));
        return new string(text);
    }
}
