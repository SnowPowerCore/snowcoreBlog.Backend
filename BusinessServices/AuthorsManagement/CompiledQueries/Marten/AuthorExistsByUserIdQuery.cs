using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.Core.Entities.Author;

namespace snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;

public class AuthorExistsByUserIdQuery : ICompiledQuery<AuthorEntity, bool>
{
    public required Guid UserId { get; set; }

    public Expression<Func<IMartenQueryable<AuthorEntity>, bool>> QueryIs()
    {
        return q => q.Any(x => x.UserId == UserId);
    }
}
