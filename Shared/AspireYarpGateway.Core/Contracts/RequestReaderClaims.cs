namespace snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

public sealed record RequestReaderClaims
{
    public required System.Guid RequestId { get; init; }
    public required System.Guid UserId { get; init; }
    public string? Email { get; init; }
    public string? TargetServiceName { get; init; }
}
