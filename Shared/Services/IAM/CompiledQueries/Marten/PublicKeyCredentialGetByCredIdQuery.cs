using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class PublicKeyCredentialGetByCredIdQuery : ICompiledQuery<Fido2PublicKeyCredentialEntity, Fido2PublicKeyCredentialEntity>
{
    public required string PublicKeyCredentialId { get; set; }

    public Expression<Func<IMartenQueryable<Fido2PublicKeyCredentialEntity>, Fido2PublicKeyCredentialEntity>> QueryIs()
    {
        return q => q.FirstOrDefault(x => x.PublicKeyCredentialId.Equals(PublicKeyCredentialId, StringComparison.Ordinal));
    }
}