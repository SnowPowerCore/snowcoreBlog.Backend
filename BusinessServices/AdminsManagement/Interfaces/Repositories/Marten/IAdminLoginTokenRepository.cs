using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;

public interface IAdminLoginTokenRepository : IRepository<AdminLoginTokenEntity>
{
    Task<AdminLoginTokenEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
}
