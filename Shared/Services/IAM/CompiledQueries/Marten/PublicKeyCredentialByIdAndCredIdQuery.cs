using System.Linq.Expressions;
using Marten.Linq;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.CompiledQueries.Marten;

public class PublicKeyCredentialByIdAndCredIdQuery : ICompiledQuery<Fido2PublicKeyCredentialEntity, bool>
{
    public required Guid Id { get; set; }

    public required string PublicKeyCredentialId { get; set; }

    public Expression<Func<IMartenQueryable<Fido2PublicKeyCredentialEntity>, bool>> QueryIs()
    {
        return q => q.Any(x => x.Id == Id && x.PublicKeyCredentialId.Equals(PublicKeyCredentialId, StringComparison.Ordinal));
    }
}