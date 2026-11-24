using Marten;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Repositories.Marten;

public class AdminInviteRepository(IDocumentSession session) : BaseMartenRepository<AdminInviteEntity>(session), IAdminInviteRepository
{
    public Task<AdminInviteEntity?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default) =>
        session.Query<AdminInviteEntity>()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

    public Task<bool> AnyUsableByEmailAsync(string email, CancellationToken ct = default) =>
        session.Query<AdminInviteEntity>()
            .AnyAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && x.IsUsable, ct);

    public async Task<List<AdminInviteEntity>> ListLatestAsync(int take, CancellationToken ct = default)
    {
        var items = await session.Query<AdminInviteEntity>()
            .OrderByDescending(static x => x.CreatedAtUtc)
            .Take(take)
            .ToListAsync(ct);

        return items.ToList();
    }
}
