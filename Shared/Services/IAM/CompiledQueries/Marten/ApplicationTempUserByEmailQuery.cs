using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class ApplicationTempUserByEmailQuery : ICompiledQuery<ApplicationTempUserEntity, bool>
{
    public required string Email { get; set; }

    public Expression<Func<IMartenQueryable<ApplicationTempUserEntity>, bool>> QueryIs()
    {
        return q => q.Any(x => x.Email == Email);
    }
}