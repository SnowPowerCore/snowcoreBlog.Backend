using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class PublicKeyCredentialByIdsAndCredIdQuery : ICompiledQuery<Fido2PublicKeyCredentialEntity, bool>
{
    public required Guid[] Ids { get; set; }

    public required byte[] PublicKeyCredentialId { get; set; }

    public Expression<Func<IMartenQueryable<Fido2PublicKeyCredentialEntity>, bool>> QueryIs()
    {
        return q => q.Any(x => Ids.Contains(x.Id) && x.PublicKeyCredentialId == PublicKeyCredentialId);
    }
}