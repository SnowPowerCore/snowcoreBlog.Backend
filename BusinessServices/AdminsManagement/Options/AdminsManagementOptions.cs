namespace snowcoreBlog.Backend.AdminsManagement.Options;

public record AdminsManagementOptions
{
    /// <summary>Email of the bootstrap (first ever) admin, seeded on launch when the store is empty.</summary>
    public string BootstrapAdminEmail { get; set; } = "admin@snowcore.blog";

    public string BootstrapAdminNickName { get; set; } = "root";

    /// <summary>Admin portal base URL used to build invite/login links.</summary>
    public string AdminFrontendDomain { get; set; } = "https://localhost/admin";

    public uint AccessTokenValidityDurationInMinutes { get; set; } = 480;

    public uint LoginLinkValidityDurationInMinutes { get; set; } = 15;

    public uint InviteValidityDurationInDays { get; set; } = 7;
}