using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.Core.Entities.Author;

namespace snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;

public class AuthorGetByUserIdQuery : ICompiledQuery<AuthorEntity, AuthorEntity>
{
    public required Guid UserId { get; set; }

    public Expression<Func<IMartenQueryable<AuthorEntity>, AuthorEntity>> QueryIs()
    {
        return q => q.FirstOrDefault(x => x.UserId == UserId)!;
    }
}
