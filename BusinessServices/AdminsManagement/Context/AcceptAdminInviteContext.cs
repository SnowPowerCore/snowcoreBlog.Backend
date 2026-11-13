using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Context;

public class AcceptAdminInviteContext(string token, string? nickName) : BaseGenericContext
{
    public string Token { get; private set; } = token;

    public string? RequestedNickName { get; } = nickName;

    public string EffectiveNickName { get; set; } = string.Empty;

    public Guid CreatedIamUserId { get; set; }

    public Core.Entities.Admin.AdminInviteEntity? Invite { get; set; }

    public Core.Entities.Admin.AdminEntity? Admin { get; set; }

    public void ClearSensitiveData()
    {
        Token = string.Empty;
    }
}
