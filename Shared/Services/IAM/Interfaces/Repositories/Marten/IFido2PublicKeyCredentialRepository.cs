using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.IAM.Core.Entities;
using System.Collections.Generic;

namespace snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;

public interface IFido2PublicKeyCredentialRepository : IRepository<Fido2PublicKeyCredentialEntity>
{
    Task<bool> CheckPublicKeyCredExistsAsync(IReadOnlyList<Guid> ids, string publicKeyCredentialId, CancellationToken token = default);

    Task<IEnumerable<Fido2PublicKeyCredentialEntity>> GetAllByUserIdAsync(Guid userId, CancellationToken token = default);
    
    Task<Fido2PublicKeyCredentialEntity> GetByUserIdAndPubKeyCredIdAsync(Guid userId, string publicKeyCredentialId, CancellationToken token = default);
}