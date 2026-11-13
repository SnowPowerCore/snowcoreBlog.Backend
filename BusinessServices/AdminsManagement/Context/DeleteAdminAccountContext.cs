using MinimalStepifiedSystem.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Context;

public class DeleteAdminAccountContext(Guid targetId, Guid currentAdminId) : BaseGenericContext
{
    public Guid TargetId { get; } = targetId;

    public Guid CurrentAdminId { get; } = currentAdminId;

    public Core.Entities.Admin.AdminEntity? Admin { get; set; }

    public bool IsActive => Admin?.IsActive ?? false;
}
