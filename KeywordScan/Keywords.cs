namespace KeywordScan;

/// <summary>
/// The keyword list the whole demo scans for: markers that suggest a request
/// body or a log line is carrying a credential. Order is fixed so that
/// Take(8) is always the same eight keywords.
/// </summary>
public static class Keywords
{
    public static readonly string[] All =
    [
        "password", "secret", "token", "apikey", "api_key", "private key",
        "credential", "passwd", "authorization", "bearer", "session",
        "connectionstring", "pwd=", "client_secret", "refresh_token",
        "access_token", "ssh-rsa", "aws_secret", "azure_storage", "sas_token",
        "x-api-key", "jdbc:", "mongodb://", "postgres://", "redis://",
        "amqp://", "smtp://", "user id=", "integrated security",
        "trustservercertificate", "encrypt=false", "keystore", "truststore",
        "pkcs12", "keyvault", "vault_token", "service_account", "privatekey",
        "clientsecret", "sharedaccesskey",
    ];

    /// <summary>
    /// The same forty keywords, each with a three-character prefix that does
    /// not occur anywhere in <see cref="SampleText"/>. Same count, same
    /// scanners, and the SIMD prefilter inside SearchValues now has nothing to
    /// stop on: this is the control for "the keyword count is not what costs".
    /// </summary>
    public static readonly string[] AllWithRarePrefixes =
        [.. All.Select((keyword, i) => "Zq" + (char)('0' + i % 10) + keyword)];

    /// <summary>The first <paramref name="count"/> keywords of <see cref="All"/>.</summary>
    public static string[] Take(int count) => All[..count];
}
