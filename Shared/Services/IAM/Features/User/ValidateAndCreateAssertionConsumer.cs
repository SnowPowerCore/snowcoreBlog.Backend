using Fido2NetLib;
using Fido2NetLib.Objects;
using Marten;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using snowcoreBlog.Backend.IAM.Constants;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateAndCreateAssertionConsumer(IFido2 fido2,
                                                UserManager<ApplicationUserEntity> userManager) : IConsumer<ValidateAndCreateAssertion>
{
    public async Task Consume(ConsumeContext<ValidateAndCreateAssertion> context)
    {
        var normalizedEmail = userManager.NormalizeEmail(context.Message.Email);

        var creds = new Dictionary<Guid, Fido2PublicKeyCredentialEntity>();
        var sourceCredentials = await userManager.Users
            .Where(user => user.NormalizedEmail == normalizedEmail)
            .Include(creds).On(x => x.PublicKeyCredentials)
            .ToListAsync(context.CancellationToken);

        var allowedCredentials = creds
            .Select(credential => new PublicKeyCredentialDescriptor(Base64UrlEncoder.DecodeBytes(credential.Value.PublicKeyCredentialId)))
            .ToList();

        if (allowedCredentials is default(List<PublicKeyCredentialDescriptor>))
        {
            await context.RespondAsync(
                new DataResult<AssertionOptions>(
                    Errors: [new(nameof(allowedCredentials), AssertionConstants.AllowedCredentialsNotFoundError)]));
            return;
        }

        var extensions = new AuthenticationExtensionsClientInputs
        {
            Extensions = true,
            UserVerificationMethod = true,
        };

        await context.RespondAsync(new DataResult<AssertionOptions>(fido2.GetAssertionOptions(
            allowedCredentials.ToGetAssertionOptionsParams(context.Message.UserVerification, extensions))));
    }
}