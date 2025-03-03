namespace snowcoreBlog.Backend.YarpGateway.Core.Contracts;

public sealed record UserTokenPairWithPayloadGenerated
{
    public required string AccessToken { get; init; }

    public required DateTimeOffset AccessTokenExpiresAt { get; init; }

    public required string RefreshToken { get; init; }

    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }
}