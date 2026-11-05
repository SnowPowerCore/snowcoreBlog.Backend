using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Services;

/// <summary>
/// Mints admin access tokens (gateway AdminScheme: issuer "admin", admin
/// signing key) and attaches the admin auth cookie. Unlike readers, admins
/// have NO rotating refresh token in this prototype — the access cookie is
/// the single credential and is simply re-obtained via a fresh login link.
/// Token creation mirrors the gateway JwtTokenCreator symmetric branch.
/// </summary>
public sealed class AdminTokenPairIssuer(
    IHttpContextAccessor httpContextAccessor,
    IOptions<AdminIssuerSigningKeyOptions> signingKey,
    IOptions<AdminsManagementOptions> options)
{
    public string IssueAndSetCookie(AdminEntity admin)
    {
        var expiresAt = DateTime.UtcNow.AddMinutes(options.Value.AccessTokenValidityDurationInMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = AdminSecurityConstants.AdminIssuer,
            IssuedAt = DateTime.UtcNow,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKey.Value.Value)),
                SecurityAlgorithms.HmacSha256Signature),
            Subject = new ClaimsIdentity(
            [
                new Claim(AdminClaimConstants.AdminUserIdClaimKey, admin.Id.ToString()),
                new Claim(AdminClaimConstants.AdminEmailClaimKey, admin.Email),
                new Claim(AdminClaimConstants.AdminAccountClaimKey, AdminClaimConstants.AdminAccountClaimValue),
            ]),
        };

        var token = new JsonWebTokenHandler().CreateToken(descriptor);

        httpContextAccessor.HttpContext?.Response.Cookies.Append(
            AuthCookieConstants.AdminAccessTokenCookieName,
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
            });

        return token;
    }

    public void ClearCookie()
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(AuthCookieConstants.AdminAccessTokenCookieName);
    }
}

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
