using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Core.Entities.Admin;

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
        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(options.Value.AccessTokenValidityDurationInMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = AdminSecurityConstants.AdminIssuer,
            IssuedAt = now,
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
