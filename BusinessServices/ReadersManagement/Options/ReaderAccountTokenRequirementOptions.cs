using MinimalStepifiedSystem.Utils;

namespace snowcoreBlog.Backend.ReadersManagement.Options;

public record ReaderAccountTokenRequirementOptions
{
    public List<string> Permissions { get; set; } = [];

    public List<string> Roles { get; set; } = [];

    public DictionaryWithDefault<string, string> Claims { get; set; } = [];

    public required uint AccessTokenValidityDurationInMinutes { get; set; }

    public required uint RefreshTokenValidityDurationInMinutes { get; set; }
}