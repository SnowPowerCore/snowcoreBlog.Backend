using Fido2NetLib;
using FluentValidation;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using MaybeResults;
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
        var loginMsg = context.Message;
        var ctxCancellationToken = context.CancellationToken;

        var result = await validator.ValidateAsync(loginMsg, ctxCancellationToken);
        if (!result.IsValid)
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: result.Errors.Select(e => new NoneDetail(e.PropertyName, e.ErrorMessage)).ToList()));
            return;
        }
        
        var loginMsgAuthAssertion = loginMsg.AuthenticatorAssertion;

        var options = AssertionOptions.FromJson(loginMsg.AssertionOptionsJson);

        var credentials = await userManager.Users
            .SelectMany(user => user.PublicKeyCredentials)
            .ToListAsync(ctxCancellationToken);

        var targetCredential = credentials.SingleOrDefault(credential =>
            credential.PublicKeyCredentialId.SequenceEqual(loginMsgAuthAssertion.Id));

        if (targetCredential is default(Fido2PublicKeyCredentialEntity))
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(targetCredential), AssertionConstants.AuthCredentialNotFoundError)]));
            return;
        }

        var user = await userManager.FindByIdAsync(targetCredential.UserId.ToString());
        if (user is default(ApplicationUserEntity))
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(user), AssertionConstants.UserNotFoundError)]));
            return;
        }

        async Task<bool> IsUserHandleOwnerOfCredentialIdCallback(Fido2NetLib.Objects.IsUserHandleOwnerOfCredentialIdParams @params, CancellationToken cancellationToken)
        {
            var creds = await userManager.Users
                .Where(user => user.Id == new Guid(@params.UserHandle).ToString())
                .SelectMany(user => user.PublicKeyCredentials)
                .ToListAsync(ctxCancellationToken);

            return creds.Any(credential => credential.PublicKeyCredentialId.SequenceEqual(@params.CredentialId));
        }

        var assertionResult = await fido2.MakeAssertionAsync(
            loginMsgAuthAssertion.ToMakeAssertionParams(
                options,
                targetCredential.PublicKey,
                targetCredential.SignatureCounter,
                IsUserHandleOwnerOfCredentialIdCallback),
            ctxCancellationToken);

        targetCredential = targetCredential with
        {
            SignatureCounter = assertionResult.SignCount,
            DevicePublicKeys = credentials.SelectMany(x => x.DevicePublicKeys).ToList()
        };

        var pubKeyCredsUpdateTask = fido2PublicKeyCredentialRepository.AddOrUpdateAsync(
            targetCredential, targetCredential.Id, token: ctxCancellationToken);
        var responseTask = context.RespondAsync(
            new DataResult<UserLoginResult>(new() { Id = new Guid(user.Id) }));

        await Task.WhenAll(pubKeyCredsUpdateTask, responseTask);
    }
}