using Marten;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Repositories.Marten;

public class AdminLoginTokenRepository(IDocumentSession session) : BaseMartenRepository<AdminLoginTokenEntity>(session), IAdminLoginTokenRepository
{
    public Task<AdminLoginTokenEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
        session.Query<AdminLoginTokenEntity>()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);
}
