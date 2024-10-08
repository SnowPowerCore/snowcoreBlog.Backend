namespace snowcoreBlog.Backend.Push.Core.Interfaces.Identity;

public interface IClaimsUser
{
    IList<string> RoleClaims { get; set; }
}