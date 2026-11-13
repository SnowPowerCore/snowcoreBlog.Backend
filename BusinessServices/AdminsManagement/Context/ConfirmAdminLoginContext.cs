using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Context;

public class ConfirmAdminLoginContext(string token) : BaseGenericContext
{
    public string Token { get; } = token;

    public Core.Entities.Admin.AdminLoginTokenEntity? LoginToken { get; set; }

    public Core.Entities.Admin.AdminEntity? Admin { get; set; }
}
