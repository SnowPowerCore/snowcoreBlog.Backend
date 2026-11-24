namespace snowcoreBlog.Backend.ReadersManagement.Core.Options;

public record ProjectOptions
{
    public string PublicBackendDomain { get; set; } = string.Empty;
    
    public string PublicFrontendDomain { get; set; } = string.Empty;
}