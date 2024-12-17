using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;

public interface IFido2PublicKeyCredentialRepository : IRepository<Fido2PublicKeyCredentialEntity>
{
}