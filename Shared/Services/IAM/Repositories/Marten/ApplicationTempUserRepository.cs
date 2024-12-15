using Marten;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.QueryProvider;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.IAM.Repositories.Marten;

public class ApplicationTempUserRepository(IDocumentSession session) : BaseMartenRepository<ApplicationTempUserEntity>(session), IApplicationTempUserRepository
{
    public Task<bool> CheckTempUserExistsByEmailAsync(string email, CancellationToken token = default) =>
        AnyByQueryAsync(MartenCompiledQueryProvider<ApplicationTempUserEntity, bool>
            .Create(new ApplicationTempUserByEmailQuery { Email = email }), token);

    public Task<ApplicationTempUserEntity?> GetTempUserByEmailAsync(string email, CancellationToken token = default) =>
        GetOneByQueryAsync(MartenCompiledQueryProvider<ApplicationTempUserEntity, ApplicationTempUserEntity>
            .Create(new ApplicationGetTempUserByEmailQuery { Email = email }), token);

    public Task<bool> CheckTempUserExistsByNickNameAsync(string nickName, CancellationToken token = default) =>
        AnyByQueryAsync(MartenCompiledQueryProvider<ApplicationTempUserEntity, bool>
            .Create(new ApplicationTempUserByNickNameQuery { NickName = nickName }), token);
}