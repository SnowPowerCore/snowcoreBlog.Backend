using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ValidateReaderAccountNotExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(context.Request.ToValidateUserExists());
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return ReaderAccountAlreadyExistsError<ReaderAccountCreationResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountAlreadyExistsError);
            }
            else
            {
                return await next(context, token);
            }
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }
}