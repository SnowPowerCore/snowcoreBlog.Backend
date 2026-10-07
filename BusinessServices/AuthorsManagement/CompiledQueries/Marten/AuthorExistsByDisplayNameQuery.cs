using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.Core.Entities.Author;

namespace snowcoreBlog.Backend.AuthorsManagement.CompiledQueries.Marten;

public class AuthorExistsByDisplayNameQuery : ICompiledQuery<AuthorEntity, bool>
{
    public required string DisplayName { get; set; }

    public Expression<Func<IMartenQueryable<AuthorEntity>, bool>> QueryIs()
    {
        return q => q.Any(x => x.DisplayName == DisplayName);
    }
}
