using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;

public class ValidateReaderAccountNotExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IResult<ReaderAccountCreatedDto>>
{
    public async Task<IResult<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(context.ConfirmRequest.ToValidateUserExists());
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return ReaderAccountAlreadyExistsError<ReaderAccountCreatedDto>.Create(
                    ReaderAccountConstants.ReaderAccountAlreadyExistsError);
            }
            else
            {
                return await next(context, token);
            }
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCreateUpdateError);
        }
    }
}