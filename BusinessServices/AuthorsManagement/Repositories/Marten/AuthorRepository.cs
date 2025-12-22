using Marten;
using snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.Backend.Infrastructure.QueryProvider;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;

public class AuthorRepository(IDocumentSession session) : BaseMartenRepository<AuthorEntity>(session), IAuthorRepository
{
	public Task<AuthorEntity?> GetByUserIdAsync(Guid userId, CancellationToken token = default) =>
		GetOneByQueryAsync(MartenCompiledQueryProvider<AuthorEntity, AuthorEntity>
			.Create(new AuthorGetByUserIdQuery { UserId = userId }), token);

	public Task<bool> AnyByUserIdAsync(Guid userId, CancellationToken token = default) =>
		AnyByQueryAsync(MartenCompiledQueryProvider<AuthorEntity, bool>
			.Create(new AuthorExistsByUserIdQuery { UserId = userId }), token);
}