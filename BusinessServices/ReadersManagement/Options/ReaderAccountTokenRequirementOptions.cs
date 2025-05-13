using MinimalStepifiedSystem.Utils;

namespace snowcoreBlog.Backend.ReadersManagement.Options;

public record ReaderAccountTokenRequirementOptions
{
    public List<string> Permissions { get; set; } = [];

    public List<string> Roles { get; set; } = [];

    public Dictionary<string, string> Claims { get; set; } = new DictionaryWithDefault<string, string>(defaultValue: string.Empty);

    public required uint AccessTokenValidityDurationInMinutes { get; set; }

    public required uint RefreshTokenValidityDurationInMinutes { get; set; }
}