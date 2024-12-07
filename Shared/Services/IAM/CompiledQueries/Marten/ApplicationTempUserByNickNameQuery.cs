using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class ApplicationTempUserByNickNameQuery : ICompiledQuery<ApplicationTempUserEntity, bool>
{
    public required string NickName { get; set; }

    public Expression<Func<IMartenQueryable<ApplicationTempUserEntity>, bool>> QueryIs()
    {
        return q => q.Any(x => string.Equals(x.UserName, NickName, StringComparison.OrdinalIgnoreCase));
    }
}