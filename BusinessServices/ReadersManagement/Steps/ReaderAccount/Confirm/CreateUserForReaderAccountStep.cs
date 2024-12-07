using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateUserForReaderAccountStep(IRequestClient<CreateUser> client) : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IResult<RequestReaderAccountCreationResultDto>>
{
    public async Task<IResult<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<DataResult<UserCreationResult>>(context.Request.ToCreateTempUser(), token);
        if (response.Message.IsSuccess)
        {
            context.SetDataWith(
                ReaderAccountUserConstants.CreateTempUserForReaderAccountResult, Result.Success(response.Message.Value));

            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<RequestReaderAccountCreationResultDto>.Create(
                ReaderAccountUserConstants.TempUserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}