using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class PublicKeyCredentialGetByUserIdAndCredIdQuery : ICompiledQuery<Fido2PublicKeyCredentialEntity, Fido2PublicKeyCredentialEntity>
{
    public required Guid UserId { get; set; }

    public required IList<byte> PublicKeyCredentialId { get; set; } = [];

    public Expression<Func<IMartenQueryable<Fido2PublicKeyCredentialEntity>, Fido2PublicKeyCredentialEntity>> QueryIs()
    {
        return q => q.SingleOrDefault(x => x.UserId == UserId && x.PublicKeyCredentialId == PublicKeyCredentialId);
    }
}