using MassTransit;
using MaybeResults;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.Admin;

public class InviteAndCreateAdminConsumer(UserManager<ApplicationAdminEntity> adminManager) : IConsumer<CreateAdmin>
{
    public async Task Consume(ConsumeContext<CreateAdmin> context)
    {
        // FromAdmin == Guid.Empty marks the FIRST EVER admin bootstrap — there
        // is no inviting admin by definition, so the inviter check is skipped.
        if (context.Message.FromAdmin != Guid.Empty)
        {
            // Ensure the inviting admin exists
            var invitingAdmin = await adminManager.FindByIdAsync(context.Message.FromAdmin.ToString());
            if (invitingAdmin is default(ApplicationAdminEntity))
            {
                await context.RespondAsync(new DataResult<AdminCreationResult>(
                    Errors: [new("FromAdmin", "Inviting admin does not exist.")]
                ));
                return;
            }
        }

        // Check if the email is already taken. Responding with the EXISTING id
        // keeps the request idempotent (bootstrap re-runs, partial failures).
        var existingAdmin = await adminManager.FindByEmailAsync(context.Message.Email);
        if (existingAdmin is not default(ApplicationAdminEntity))
        {
            await context.RespondAsync(new DataResult<AdminCreationResult>(
                new() { Id = Guid.Parse(existingAdmin.Id) }
            ));
            return;
        }

        var newAdmin = new ApplicationAdminEntity
        {
            UserName = context.Message.NickName,
            Email = context.Message.Email,
            EmailConfirmed = false
        };
        var result = await adminManager.CreateAsync(newAdmin);
        if (!result.Succeeded)
        {
            await context.RespondAsync(new DataResult<AdminCreationResult>(
                Errors: result.Errors.Select(e => new NoneDetail(e.Code, e.Description)).ToList()
            ));
            return;
        }
        await context.RespondAsync(new DataResult<AdminCreationResult>(new() { Id = Guid.Parse(newAdmin.Id) }));
    }
}
