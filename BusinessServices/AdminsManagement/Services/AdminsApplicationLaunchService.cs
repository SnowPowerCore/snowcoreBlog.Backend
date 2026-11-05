using MassTransit;
using Marten;
using Microsoft.Extensions.Options;
using snowcoreBlog.ApplicationLaunch.Interfaces;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.AdminsManagement.Services;

/// <summary>
/// Seeds the FIRST EVER admin (InvitedByAdminId = null) when the store is
/// empty. The bootstrap admin is the only admin allowed to exist without an
/// inviter reference — every subsequent admin MUST be invited by an
/// existing admin (FK-enforced AdminEntity.InvitedByAdminId). The IAM admin
/// identity is created through the CreateAdmin request (FromAdmin = empty
/// marks the bootstrap path in the IAM consumer).
/// </summary>
public sealed class AdminsApplicationLaunchService(IServiceProvider serviceProvider) : IApplicationLaunchService
{
    public async Task InitAsync()
    {
        using var scope = serviceProvider.CreateScope();
        var session = scope.ServiceProvider.GetRequiredService<IDocumentSession>();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<AdminsManagementOptions>>();
        var createAdminClient = scope.ServiceProvider.GetRequiredService<IRequestClient<CreateAdmin>>();

        if (await session.Query<AdminEntity>().AnyAsync())
        {
            return;
        }

        var bootstrapEmail = options.Value.BootstrapAdminEmail.Trim().ToLowerInvariant();

        var iamResponse = await createAdminClient.GetResponse<DataResult<AdminCreationResult>>(
            new CreateAdmin
            {
                FromAdmin = Guid.Empty, // bootstrap marker — IAM skips the inviter check
                Email = bootstrapEmail,
                NickName = options.Value.BootstrapAdminNickName,
            });

        if (!iamResponse.Message.IsSuccess || iamResponse.Message.Value is default(AdminCreationResult))
        {
            throw new InvalidOperationException(
                $"Failed to create the bootstrap admin identity in IAM: {
                    string.Join("; ", iamResponse.Message.Errors?.Select(static e => $"{e.Code}: {e.Description}") ?? [])
                }");
        }

        session.Insert(new AdminEntity
        {
            Email = bootstrapEmail,
            NickName = options.Value.BootstrapAdminNickName,
            InvitedByAdminId = null, // first ever admin — the ONLY allowed null
            IamUserId = iamResponse.Message.Value.Id,
            InvitedAtUtc = DateTime.UtcNow,
            IsActive = true,
        });
        await session.SaveChangesAsync();
    }
}
