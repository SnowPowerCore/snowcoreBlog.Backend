using snowcoreBlog.Backend.Core.Interfaces.Repositories;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;

public interface IApplicationTempUserRepository : IRepository<ApplicationTempUserEntity>
{
    Task<bool> CheckTempUserExistsByEmailAsync(string email, CancellationToken token = default);

    Task<ApplicationTempUserEntity?> GetTempUserByEmailAsync(string email, CancellationToken token = default);

    Task<bool> CheckTempUserExistsByNickNameAsync(string nickName, CancellationToken token = default);
}