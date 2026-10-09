using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.OneTimeCode;

public class IssueTokenForOneTimeCodeLoginStep(ReaderAccountTokenPairIssuer tokenPairIssuer) : IStep<LoginByOneTimeCodeDelegate, LoginByOneTimeCodeContext, IMaybe<LoginByOneTimeCodeResultDto>>
{
    public async Task<IMaybe<LoginByOneTimeCodeResultDto>> InvokeAsync(LoginByOneTimeCodeContext context, LoginByOneTimeCodeDelegate next, CancellationToken token = default)
    {
        var currentUserId = context.GetFromData<Guid>(ReaderAccountUserConstants.CurrentUserId);

        var (success, errors) = await tokenPairIssuer.TryIssueAsync(currentUserId, context.VerifyOneTimeCode.Email, token);
        if (success)
        {
            return Maybe.Create<LoginByOneTimeCodeResultDto>(new());
        }
        else
        {
            return ReaderAccountNotExistsError<LoginByOneTimeCodeResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, errors);
        }
    }
}
