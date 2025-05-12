﻿using Marten;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.QueryProvider;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.IAM.Repositories.Marten;

public class Fido2PublicKeyCredentialRepository(IDocumentSession session) : BaseMartenRepository<Fido2PublicKeyCredentialEntity>(session), IFido2PublicKeyCredentialRepository
{
    public async Task<bool> CheckPublicKeyCredExistsAsync(Guid[] ids, byte[] publicKeyCredentialId, CancellationToken token = default)
    {
        var batch = session.CreateBatchQuery();
        var exists = false;
        var tasks = new List<Task<bool>>();
        foreach (var id in ids)
        {
            tasks.Add(batch.Query(new PublicKeyCredentialByIdAndCredIdQuery { Id = id, PublicKeyCredentialId = publicKeyCredentialId }));
        }
        await batch.Execute(token);
        foreach (var task in tasks)
        {
            exists = await task;
            if (exists)
                break;
        }
        return exists;
    }

    public Task<IEnumerable<Fido2PublicKeyCredentialEntity>> GetAllByUserIdAsync(Guid userId, CancellationToken token = default) =>
        GetAllByQueryAsync(MartenCompiledQueryProvider<Fido2PublicKeyCredentialEntity, IEnumerable<Fido2PublicKeyCredentialEntity>>
            .Create(new PublicKeyCredentialsGetByUserIdQuery { UserId = userId }), token);

    public Task<Fido2PublicKeyCredentialEntity> GetByUserIdAndPubKeyCredIdAsync(Guid userId, byte[] publicKeyCredentialId, CancellationToken token = default) =>
        GetOneByQueryAsync(MartenCompiledQueryProvider<Fido2PublicKeyCredentialEntity, Fido2PublicKeyCredentialEntity>
            .Create(new PublicKeyCredentialGetByUserIdAndCredIdQuery { UserId = userId, PublicKeyCredentialId = publicKeyCredentialId }), token);
}