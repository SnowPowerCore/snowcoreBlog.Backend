using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class ValidateReaderAccountExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IResult<LoginByAssertionResultDto>>
{
    public async Task<IResult<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(context.LoginByAssertion.ToValidateUserExists());
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return await next(context, token);
            }
            else
            {
                return ReaderAccountNotExistError<LoginByAssertionResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountNotExistError);
            }
        }
        else
        {
            return ReaderAccountNotExistError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError, result.Message.Errors);
        }
    }
}