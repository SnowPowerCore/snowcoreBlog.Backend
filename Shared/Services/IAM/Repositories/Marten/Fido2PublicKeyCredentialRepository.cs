using Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.IAM.Repositories.Marten;

public class Fido2PublicKeyCredentialRepository(IDocumentSession session) : BaseMartenRepository<Fido2PublicKeyCredentialEntity>(session), IFido2PublicKeyCredentialRepository
{
}