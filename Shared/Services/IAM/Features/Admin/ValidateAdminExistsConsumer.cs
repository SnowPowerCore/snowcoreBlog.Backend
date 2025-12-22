using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.Admin;

public class ValidateAdminExistsConsumer(UserManager<ApplicationAdminEntity> adminManager) : IConsumer<ValidateAdminExists>
{
    public async Task Consume(ConsumeContext<ValidateAdminExists> context)
    {
        var admin = await adminManager.FindByEmailAsync(adminManager.NormalizeEmail(context.Message.Email));
        if (admin is not default(ApplicationAdminEntity))
        {
            await context.RespondAsync(
                new DataResult<AdminExistsValidationResult>(new() { Exists = true }));
        }
        else
        {
            await context.RespondAsync(
                new DataResult<AdminExistsValidationResult>(new()));
        }
    }
}