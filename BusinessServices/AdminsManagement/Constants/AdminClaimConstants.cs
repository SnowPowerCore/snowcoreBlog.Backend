namespace snowcoreBlog.Backend.AdminsManagement.Constants;

/// <summary>
/// JWT claim keys for admin tokens. The <see cref="AdminAccountClaimKey"/>
/// value must match the gateway `regularAdmin` policy requirement.
/// </summary>
public static class AdminClaimConstants
{
    public const string AdminEmailClaimKey = "AdminEmail";
    public const string AdminUserIdClaimKey = "AdminUserId";
    public const string AdminAccountClaimKey = "adminAccount";
    public const string AdminAccountClaimValue = "true";
}