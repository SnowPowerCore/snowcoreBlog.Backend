using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;

public interface IFido2PublicKeyCredentialRepository : IRepository<Fido2PublicKeyCredentialEntity>
{
    Task<bool> CheckPublicKeyCredExistsAsync(Guid[] ids, byte[] publicKeyCredentialId, CancellationToken token = default);

    Task<IEnumerable<Fido2PublicKeyCredentialEntity>> GetAllByUserIdAsync(Guid userId, CancellationToken token = default);
    
    Task<Fido2PublicKeyCredentialEntity> GetByUserIdAndPubKeyCredIdAsync(Guid userId, byte[] publicKeyCredentialId, CancellationToken token = default);
}