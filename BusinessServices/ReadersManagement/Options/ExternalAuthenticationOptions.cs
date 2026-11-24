namespace snowcoreBlog.Backend.ReadersManagement.Options;

public sealed class ExternalAuthenticationOptions
{
    public ExternalProviderOptions Microsoft { get; init; } = new();

    public ExternalProviderOptions Google { get; init; } = new();

    public AppleExternalProviderOptions Apple { get; init; } = new();
}
