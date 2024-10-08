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

public class CreateUserForReaderAccountStep(IRequestClient<CreateUser> client) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<DataResult<UserCreationResult>>(context.Request.ToCreateUser(), token);
        if (response.Message.IsSuccess)
        {
            context.SetDataWith(
                ReaderAccountUserConstants.CreateUserForReaderAccountResult, Result.Success(response.Message.Value));

            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreationResultDto>.Create(
                ReaderAccountUserConstants.UserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}