using Fido2NetLib;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Results;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.Backend.IAM.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class CheckAndPerformAssertionConsumer(IFido2 fido2,
                                              IValidator<LoginUser> validator,
                                              IFido2PublicKeyCredentialRepository fido2PublicKeyCredentialRepository,
                                              UserManager<ApplicationUserEntity> userManager) : IConsumer<LoginUser>
{
    public async Task Consume(ConsumeContext<LoginUser> context)
    {
        var result = await validator.ValidateAsync(context.Message, context.CancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: result.Errors.Select(e => new ErrorResultDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }

        var options = AssertionOptions.FromJson(context.Message.AssertionOptionsJson);

        var credential = await userManager.Users
            .SelectMany(user => user.PublicKeyCredentials)
            .Include(credential => credential.DevicePublicKeys)
            .SingleOrDefaultAsync(credential => credential.Id == new Guid(context.Message.AuthenticatorAssertion.Id), context.CancellationToken);

        if (credential is default(Fido2PublicKeyCredentialEntity))
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(credential), AssertionConstants.AuthCredentialNotFoundError)]));
            return;
        }

        var user = await userManager.FindByIdAsync(credential.UserId.ToString());
        if (user is default(ApplicationUserEntity))
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(user), AssertionConstants.UserNotFoundError)]));
            return;
        }

        Task<bool> IsUserHandleOwnerOfCredentialIdCallback(Fido2NetLib.Objects.IsUserHandleOwnerOfCredentialIdParams @params, CancellationToken cancellationToken) =>
            userManager.Users
                .Where(user => user.Id == new Guid(@params.UserHandle).ToString())
                .SelectMany(user => user.PublicKeyCredentials)
                .AnyAsync(credential => credential.Id == new Guid(@params.CredentialId), context.CancellationToken);

        var assertionResult = await fido2.MakeAssertionAsync(
            context.Message.AuthenticatorAssertion.ToMakeAssertionParams(
                options,
                credential.PublicKey,
                credential.SignatureCounter,
                IsUserHandleOwnerOfCredentialIdCallback),
            context.CancellationToken);

        credential = credential with { SignatureCounter = assertionResult.SignCount };

        var pubKeyCredsUpdateTask = fido2PublicKeyCredentialRepository.AddOrUpdateAsync(
            credential, credential.Id, token: context.CancellationToken);
        var responseTask = context.RespondAsync(
            new DataResult<UserLoginResult>(new UserLoginResult() { Id = new Guid(user.Id) }));

        await Task.WhenAll(pubKeyCredsUpdateTask, responseTask);
    }
}