using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;

public interface IAdminInviteRepository : IRepository<AdminInviteEntity>
{
    Task<AdminInviteEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);

    Task<bool> AnyUsableByEmailAsync(string email, CancellationToken ct = default);

    Task<List<AdminInviteEntity>> ListLatestAsync(int take, CancellationToken ct = default);
}
