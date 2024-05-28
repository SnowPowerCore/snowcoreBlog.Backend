namespace snowcoreBlog.Backend.IAM.Core.Interfaces.Identity
{
    public interface IClaimsUser
    {
        IList<string> RoleClaims { get; set; }
    }
}