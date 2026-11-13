using System.Security.Claims;
using snowcoreBlog.Backend.AdminsManagement.Constants;

namespace snowcoreBlog.Backend.AdminsManagement.Extensions;

/// <summary>Helpers to read the admin from the current principal (admin-scheme JWT).</summary>
public static class AdminPrincipalExtensions
{
    public static bool IsAdmin(this ClaimsPrincipal principal)
        => principal.HasClaim(AdminClaimConstants.AdminAccountClaimKey, AdminClaimConstants.AdminAccountClaimValue);

    public static Guid GetAdminId(this ClaimsPrincipal principal)
        => Guid.TryParse(principal.FindFirst(AdminClaimConstants.AdminUserIdClaimKey)?.Value, out var id) ? id : Guid.Empty;

    public static string GetAdminEmail(this ClaimsPrincipal principal)
        => principal.FindFirst(AdminClaimConstants.AdminEmailClaimKey)?.Value ?? string.Empty;
}
