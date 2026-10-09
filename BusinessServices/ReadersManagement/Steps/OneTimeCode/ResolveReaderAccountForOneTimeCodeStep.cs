using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.OneTimeCode;

public class ResolveReaderAccountForOneTimeCodeStep(IRequestClient<ValidateUserExists> requestClient) : IStep<LoginByOneTimeCodeDelegate, LoginByOneTimeCodeContext, IMaybe<LoginByOneTimeCodeResultDto>>
{
    public async Task<IMaybe<LoginByOneTimeCodeResultDto>> InvokeAsync(LoginByOneTimeCodeContext context, LoginByOneTimeCodeDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(
            new ValidateUserExists { Email = context.VerifyOneTimeCode.Email }, token);
        if (result.Message.IsSuccess)
        {
            var value = result.Message.Value;
            if (value is { Exists: true } && value.UserId is { } userId)
            {
                context.SetDataWith(ReaderAccountUserConstants.CurrentUserId, userId);

                return await next(context, token);
            }
            else
            {
                return ReaderAccountNotExistsError<LoginByOneTimeCodeResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountNotExistsError);
            }
        }
        else
        {
            return ReaderAccountNotExistsError<LoginByOneTimeCodeResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError, result.Message.Errors);
        }
    }
}
