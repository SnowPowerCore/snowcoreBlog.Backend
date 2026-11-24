using Fido2NetLib;
using Fido2NetLib.Objects;
using MassTransit;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.IAM.Features.User;

public class ValidateAndCreateAssertionConsumer(IFido2 fido2) : IConsumer<ValidateAndCreateAssertion>
{
    public async Task Consume(ConsumeContext<ValidateAndCreateAssertion> context)
    {
        // Usernameless (discoverable-credential) assertion: no allowCredentials filter, so the
        // native dialog may offer any passkey registered for this site — even one belonging to
        // a different account than the email entered in the login form. Identity is resolved
        // from the chosen credential's owner when the assertion is verified.
        var allowedCredentials = new List<PublicKeyCredentialDescriptor>();

        var extensions = new AuthenticationExtensionsClientInputs
        {
            Extensions = true,
            UserVerificationMethod = true,
        };

        await context.RespondAsync(new DataResult<AssertionOptions>(fido2.GetAssertionOptions(
            allowedCredentials.ToGetAssertionOptionsParams(context.Message.UserVerification, extensions))));
    }
}