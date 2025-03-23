using FastEndpoints.Security;

namespace snowcoreBlog.Backend.AspireYarpGateway.Options;

public record SecuritySigningOptions
{
    public JwtCreationOptions User { get; set; }

    public JwtCreationOptions Admin { get; set; }
}