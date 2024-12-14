using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class ValidateAndCreateReaderAccountUserStep(IRequestClient<CreateUser> client,
                                                    IPublishEndpoint publishEndpoint) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IResult<ReaderAccountCreatedDto>>
{
    public async Task<IResult<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var response = await client.GetResponse<DataResult<UserCreationResult>>(context.Request.ToCreateUser(), token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response!.Message.Value;

            await publishEndpoint.Publish<ReaderAccountUserCreated>(
                new(responseObj!.FirstName,
                    responseObj.Email), token);

            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountUserConstants.UserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}