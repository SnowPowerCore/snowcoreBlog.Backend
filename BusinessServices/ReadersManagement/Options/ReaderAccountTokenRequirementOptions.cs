using snowcoreBlog.PublicApi.Utilities.Dictionary;

namespace snowcoreBlog.Backend.ReadersManagement.Options;

public record ReaderAccountTokenRequirementOptions
{
    public List<string> Permissions { get; set; } = [];

    public List<string> Roles { get; set; } = [];

    public Dictionary<string, string> Claims { get; set; } = new DictionaryWithDefault<string, string>(defaultValue: string.Empty, 0);

    public required uint AccessTokenValidityDurationInMinutes { get; set; }

    public required uint RefreshTokenValidityDurationInMinutes { get; set; }

    // List of concrete microservice names that should be queried for additional claims
    public List<string> ClaimProviderServices { get; set; } = [];
}