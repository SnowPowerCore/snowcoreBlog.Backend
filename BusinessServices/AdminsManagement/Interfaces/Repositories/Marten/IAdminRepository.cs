using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;

public interface IAdminRepository : IRepository<AdminEntity>
{
    Task<AdminEntity?> GetActiveByEmailAsync(string email, CancellationToken ct = default);

    Task<bool> AnyByEmailAsync(string email, CancellationToken ct = default);

    Task<List<AdminEntity>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct = default);

    Task<List<AdminEntity>> GetActiveExceptAsync(Guid exceptId, CancellationToken ct = default);
}
