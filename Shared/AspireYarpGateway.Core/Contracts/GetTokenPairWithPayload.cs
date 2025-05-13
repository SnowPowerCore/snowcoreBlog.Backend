using snowcoreBlog.PublicApi.Utilities.Dictionary;

namespace snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

public record GetTokenPairWithPayload
{
    public required List<string> Permissions { get; init; } = [];
    
    public required List<string> Roles { get; init; } = [];
    
    public required Dictionary<string, string> Claims { get; init; } = new DictionaryWithDefault<string, string>(defaultValue: string.Empty);

    public required uint AccessTokenValidityDurationInMinutes { get; init; } = 1;
    
    public required uint RefreshTokenValidityDurationInMinutes { get; init; } = 1;
}