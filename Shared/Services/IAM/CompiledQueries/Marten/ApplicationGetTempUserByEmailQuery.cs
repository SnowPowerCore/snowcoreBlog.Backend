using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class ApplicationGetTempUserByEmailQuery : ICompiledQuery<ApplicationTempUserEntity, ApplicationTempUserEntity>
{
    public required string Email { get; set; }

    public Expression<Func<IMartenQueryable<ApplicationTempUserEntity>, ApplicationTempUserEntity>> QueryIs()
    {
        return q => q.FirstOrDefault(x => x.Email == Email)!;
    }
}