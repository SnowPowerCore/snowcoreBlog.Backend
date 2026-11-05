namespace snowcoreBlog.Backend.AdminsManagement.Constants;

/// <summary>
/// MUST stay in sync with the gateway's AdminScheme ValidIssuer
/// (AspireYarpGateway AddMultipleAuthentications) — admin JWTs are
/// validated by the gateway under that scheme.
/// </summary>
public static class AdminSecurityConstants
{
    public const string AdminIssuer = "admin";
}