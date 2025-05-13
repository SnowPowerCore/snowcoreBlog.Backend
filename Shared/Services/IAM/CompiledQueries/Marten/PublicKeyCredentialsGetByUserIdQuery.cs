using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class PublicKeyCredentialsGetByUserIdQuery : ICompiledListQuery<Fido2PublicKeyCredentialEntity, Fido2PublicKeyCredentialEntity>
{
    public required Guid UserId { get; set; }

    public Expression<Func<IMartenQueryable<Fido2PublicKeyCredentialEntity>, IEnumerable<Fido2PublicKeyCredentialEntity>>> QueryIs()
    {
        return q => q.Where(x => x.UserId == UserId);
    }
}