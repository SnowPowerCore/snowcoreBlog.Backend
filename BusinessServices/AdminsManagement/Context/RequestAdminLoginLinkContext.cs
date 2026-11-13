using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Context;

public class RequestAdminLoginLinkContext(string email) : BaseGenericContext
{
    public string Email { get; } = email;

    public string? RawToken { get; set; }

    public Core.Entities.Admin.AdminEntity? Admin { get; set; }
}
