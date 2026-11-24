using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;

public interface IAdminLoginTokenRepository : IRepository<AdminLoginTokenEntity>
{
    Task<AdminLoginTokenEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
}
