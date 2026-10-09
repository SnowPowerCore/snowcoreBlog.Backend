using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.OneTimeCode;

public class VerifyAuthenticatorOneTimeCodeStep(IRequestClient<ValidateUserOneTimeCode> requestClient) : IStep<LoginByOneTimeCodeDelegate, LoginByOneTimeCodeContext, IMaybe<LoginByOneTimeCodeResultDto>>
{
    public async Task<IMaybe<LoginByOneTimeCodeResultDto>> InvokeAsync(LoginByOneTimeCodeContext context, LoginByOneTimeCodeDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserLoginResult>>(
            new ValidateUserOneTimeCode
            {
                Email = context.VerifyOneTimeCode.Email,
                Code = context.VerifyOneTimeCode.Code
            }, token);
        if (result.Message.IsSuccess)
        {
            return await next(context, token);
        }
        else
        {
            return OneTimeCodeError<LoginByOneTimeCodeResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, result.Message.Errors);
        }
    }
}
