using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Context;

public class CreateAdminInviteContext(CreateAdminInviteDto request, Guid invitedByAdminId, string invitedByAdminEmail) : BaseGenericContext
{
    public CreateAdminInviteDto Request { get; } = request;

    public Guid InvitedByAdminId { get; } = invitedByAdminId;

    public string InvitedByAdminEmail { get; } = invitedByAdminEmail;

    public string Email { get; } = request.Email.Trim().ToLowerInvariant();

    public Core.Entities.Admin.AdminInviteEntity? Invite { get; set; }

    public string? RawToken { get; set; }
}
