using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Identity;

namespace snowcoreBlog.Backend.Push.Entities;

public class ApplicationUser : IdentityUser, IClaimsUser
{
    public IList<string> RoleClaims { get; set; }
}