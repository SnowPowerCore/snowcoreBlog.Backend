using System.Security.Claims;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

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

    public static AdminDto ToDto(this AdminEntity admin)
        => new()
        {
            Id = admin.Id,
            Email = admin.Email,
            NickName = admin.NickName,
            InvitedByAdminId = admin.InvitedByAdminId,
            InvitedAtUtc = admin.InvitedAtUtc,
            IsActive = admin.IsActive,
        };
}
