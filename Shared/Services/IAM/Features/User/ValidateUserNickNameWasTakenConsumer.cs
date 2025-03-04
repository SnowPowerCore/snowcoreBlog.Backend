﻿using MassTransit;
using Microsoft.AspNetCore.Identity;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateUserNickNameWasTakenConsumer(UserManager<ApplicationUserEntity> userManager,
                                                  IApplicationTempUserRepository applicationTempUserRepository) : IConsumer<ValidateUserNickNameTaken>
{
    public async Task Consume(ConsumeContext<ValidateUserNickNameTaken> context)
    {
        var user = await userManager.FindByNameAsync(userManager.NormalizeName(context.Message.NickName));
        if (user is not default(ApplicationUserEntity))
        {
            await context.RespondAsync(
                new DataResult<UserNickNameTakenValidationResult>(new() { WasTaken = true }));
        }

        var userExists = await applicationTempUserRepository.CheckTempUserExistsByNickNameAsync(context.Message.NickName);
        if (userExists)
        {
            await context.RespondAsync(
                new DataResult<UserNickNameTakenValidationResult>(new() { WasTaken = true }));
        }

        await context.RespondAsync(new DataResult<UserNickNameTakenValidationResult>(new()));
    }
}