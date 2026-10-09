using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Identity;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public class ApplicationUserEntity : IdentityUser, IClaimsUser
{
    public IList<string> RoleClaims { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public ICollection<Guid> PublicKeyCredentials { get; set; } = [];

    /// <summary>
    /// Base32-encoded TOTP shared secret used with authenticator apps
    /// (Microsoft / Google Authenticator). Present when the account was
    /// created with, or enrolled into, authenticator-based two-factor login.
    /// </summary>
    public string? TotpSecret { get; set; }
}