using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class GetTokenForReaderAccountStep(ReaderAccountTokenPairIssuer tokenPairIssuer) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var currentUserId = context.GetFromData<Guid>(ReaderAccountUserConstants.CurrentUserId);

        // Passkey sign-ins resolve the actual account in IAM — prefer the resolved email
        // over the one entered in the login form (they may differ).
        var email = context.GetFromData<string>(ReaderAccountUserConstants.CurrentUserEmail) is { Length: > 0 } resolvedEmail
            ? resolvedEmail
            : context.LoginByAssertion.Email;

        var (success, errors) = await tokenPairIssuer.TryIssueAsync(currentUserId, email, token);
        if (success)
        {
            return Maybe.Create<LoginByAssertionResultDto>(new());
        }
        else
        {
            return ReaderAccountNotExistsError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, errors);
        }
    }
}