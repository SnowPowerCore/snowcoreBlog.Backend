﻿using Marten;
using snowcoreBlog.Backend.IAM.CompiledQueries.Marten;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure.QueryProvider;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.IAM.Repositories.Marten;

public class ApplicationTempUserRepository(IDocumentSession session) : BaseMartenRepository<ApplicationTempUserEntity>(session), IApplicationTempUserRepository
{
    public Task<bool> CheckTempUserExistsByEmailAsync(string email) =>
        AnyByQueryAsync(MartenCompiledQueryProvider<ApplicationTempUserEntity, bool>
            .Create(new ApplicationTempUserByEmailQuery { Email = email }));

    public Task<bool> CheckTempUserExistsByNickNameAsync(string nickName) =>
        AnyByQueryAsync(MartenCompiledQueryProvider<ApplicationTempUserEntity, bool>
            .Create(new ApplicationTempUserByNickNameQuery { NickName = nickName }));
}