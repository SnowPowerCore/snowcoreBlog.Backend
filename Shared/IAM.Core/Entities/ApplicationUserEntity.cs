using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Identity;

namespace snowcoreBlog.Backend.IAM.Core.Entities;

public class ApplicationUser : IdentityUser, IClaimsUser
{
    public IList<string> RoleClaims { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }
}