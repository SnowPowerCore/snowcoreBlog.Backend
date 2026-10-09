namespace snowcoreBlog.Backend.ReadersManagement.Options;

public sealed class ExternalAuthenticationOptions
{
    public ExternalProviderOptions Microsoft { get; init; } = new();

    public ExternalProviderOptions Google { get; init; } = new();

    public AppleExternalProviderOptions Apple { get; init; } = new();
}

public sealed class ExternalProviderOptions
{
    public string ClientId { get; init; } = string.Empty;

    public string ClientSecret { get; init; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(ClientSecret);
}

/// <summary>
/// Sign in with Apple does not use a shared client secret — instead an ES256
/// private key (.p8 PEM content) is used to generate short-lived client secrets.
/// </summary>
public sealed class AppleExternalProviderOptions
{
    /// <summary>The Services ID configured in the Apple developer portal.</summary>
    public string ClientId { get; init; } = string.Empty;

    public string KeyId { get; init; } = string.Empty;

    public string TeamId { get; init; } = string.Empty;

    /// <summary>The full .p8 private key PEM content.</summary>
    public string PrivateKeyPem { get; init; } = string.Empty;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(ClientId) &&
        !string.IsNullOrWhiteSpace(KeyId) &&
        !string.IsNullOrWhiteSpace(TeamId) &&
        !string.IsNullOrWhiteSpace(PrivateKeyPem);
}
