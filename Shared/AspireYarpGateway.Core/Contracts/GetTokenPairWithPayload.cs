using snowcoreBlog.PublicApi.Utilities.Dictionary;

namespace snowcoreBlog.Backend.YarpGateway.Core.Contracts;

public record GetTokenPairWithPayload
{
    public required List<string> Permissions { get; init; } = [];
    
    public required List<string> Roles { get; init; } = [];
    
    public required DictionaryWithDefault<string, string> Claims { get; init; } = new(defaultValue: string.Empty);

    public required uint AccessTokenValidityDurationInMinutes { get; init; } = 1;
    
    public required uint RefreshTokenValidityDurationInMinutes { get; init; } = 1;
}