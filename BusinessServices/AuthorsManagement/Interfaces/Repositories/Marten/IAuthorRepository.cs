using snowcoreBlog.Backend.AuthorsManagement.Core.Entities.Author;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;

public interface IAuthorRepository : IRepository<AuthorEntity>
{
	Task<AuthorEntity?> GetByUserIdAsync(Guid userId, CancellationToken token = default);

	Task<bool> AnyByUserIdAsync(Guid userId, CancellationToken token = default);

	Task<bool> AnyByDisplayNameAsync(string displayName, CancellationToken token = default);
}