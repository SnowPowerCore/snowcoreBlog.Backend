using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Identity;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public class ApplicationAdmin : IdentityUser, IClaimsUser
{
    public IList<string> RoleClaims { get; set; }
}