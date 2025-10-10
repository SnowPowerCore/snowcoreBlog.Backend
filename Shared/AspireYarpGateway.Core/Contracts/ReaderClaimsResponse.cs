namespace snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

public sealed record ReaderClaimsResponse
{
    public required System.Guid RequestId { get; init; }
    public required System.Collections.Generic.Dictionary<string, string> Claims { get; init; }
    public string? SourceService { get; init; }
}
