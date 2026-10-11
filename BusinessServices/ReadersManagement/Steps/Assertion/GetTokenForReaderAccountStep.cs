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

        var (success, errors) = await tokenPairIssuer.TryIssueAsync(currentUserId, context.LoginByAssertion.Email, token);
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