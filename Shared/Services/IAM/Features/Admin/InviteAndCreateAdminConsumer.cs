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
        // Ensure the inviting admin exists
        var invitingAdmin = await adminManager.FindByIdAsync(context.Message.FromAdmin.ToString());
        if (invitingAdmin is null)
        {
            await context.RespondAsync(new DataResult<AdminCreationResult>(
                Errors: [new("FromAdmin", "Inviting admin does not exist.")]
            ));
            return;
        }

        // Check if the email is already taken
        var existingAdmin = await adminManager.FindByEmailAsync(context.Message.Email);
        if (existingAdmin is not null)
        {
            await context.RespondAsync(new DataResult<AdminCreationResult>(
                Errors: [new("Email", "Admin with this email already exists.")]
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
