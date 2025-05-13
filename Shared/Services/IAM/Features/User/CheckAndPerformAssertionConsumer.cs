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
using Microsoft.IdentityModel.Tokens;

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

        var user = await userManager.FindByEmailAsync(userManager.NormalizeEmail(loginMsg.Email));
        if (user is default(ApplicationUserEntity))
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(user), AssertionConstants.UserNotFoundError)]));
            return;
        }

        var userIdGuid = new Guid(user.Id);

        var options = AssertionOptions.FromJson(loginMsg.AssertionOptionsJson);
        
        var targetCredential = await fido2PublicKeyCredentialRepository
            .GetByUserIdAndPubKeyCredIdAsync(userIdGuid, Base64UrlEncoder.Encode(loginMsgAuthAssertion.Id));
        if (targetCredential is default(Fido2PublicKeyCredentialEntity))
        {
            await context.RespondAsync(
                new DataResult<UserLoginResult>(
                    Errors: [new(nameof(targetCredential), AssertionConstants.AuthCredentialNotFoundError)]));
            return;
        }

        async Task<bool> IsUserHandleOwnerOfCredentialIdCallback(Fido2NetLib.Objects.IsUserHandleOwnerOfCredentialIdParams @params, CancellationToken cancellationToken)
        {
            var userId = new Guid(@params.UserHandle).ToString();
            var creds = await userManager.Users
                .Where(user => user.Id == userId)
                .SelectMany(user => user.PublicKeyCredentials)
                .ToListAsync(context.CancellationToken);
            return await fido2PublicKeyCredentialRepository
                .CheckPublicKeyCredExistsAsync(creds.ToArray(), Base64UrlEncoder.Encode(@params.CredentialId), cancellationToken);
        }

        var assertionResult = await fido2.MakeAssertionAsync(
            loginMsgAuthAssertion.ToMakeAssertionParams(
                options,
                Base64UrlEncoder.DecodeBytes(targetCredential.PublicKey),
                targetCredential.SignatureCounter,
                IsUserHandleOwnerOfCredentialIdCallback),
            ctxCancellationToken);

        var credentialsByUserId = await fido2PublicKeyCredentialRepository
            .GetAllByUserIdAsync(userIdGuid);

        targetCredential = targetCredential with
        {
            SignatureCounter = assertionResult.SignCount,
            DevicePublicKeys = credentialsByUserId.SelectMany(x => x.DevicePublicKeys).ToList()
        };

        var pubKeyCredsUpdateTask = fido2PublicKeyCredentialRepository
            .AddOrUpdateAsync(targetCredential, targetCredential.Id, token: ctxCancellationToken);
        var responseTask = context.RespondAsync(
            new DataResult<UserLoginResult>(new() { Id = userIdGuid }));

        await Task.WhenAll(pubKeyCredsUpdateTask, responseTask);
    }
}