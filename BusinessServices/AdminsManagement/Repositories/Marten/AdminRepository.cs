using Marten;
using snowcoreBlog.Backend.AdminsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AdminsManagement.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.AdminsManagement.Repositories.Marten;

public class AdminRepository(IDocumentSession session) : BaseMartenRepository<AdminEntity>(session), IAdminRepository
{
    public Task<AdminEntity?> GetActiveByEmailAsync(string email, CancellationToken ct = default) =>
        session.Query<AdminEntity>()
            .Where(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && x.IsActive)
            .FirstOrDefaultAsync(ct);

    public Task<bool> AnyByEmailAsync(string email, CancellationToken ct = default) =>
        session.Query<AdminEntity>()
            .AnyAsync(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase), ct);

    public async Task<List<AdminEntity>> SearchAsync(string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var query = session.Query<AdminEntity>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Email.Contains(term, StringComparison.OrdinalIgnoreCase)
                || x.NickName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var items = await query
            .OrderBy(static x => x.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return items.ToList();
    }

    public async Task<List<AdminEntity>> GetActiveExceptAsync(Guid exceptId, CancellationToken ct = default) =>
        await session.Query<AdminEntity>()
            .Where(static x => x.IsActive)
            .ToListAsync(ct) is var all
            ? all.Where(x => x.Id != exceptId).ToList()
            : [];
}
