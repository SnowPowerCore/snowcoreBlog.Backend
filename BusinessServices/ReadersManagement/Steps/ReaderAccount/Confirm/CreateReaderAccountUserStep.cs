using System.Text;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateReaderAccountUserStep(IRequestClient<CreateUser> client,
                                         IPublishEndpoint publishEndpoint,
                                         IDistributedCache distributedCache) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IResult<ReaderAccountCreatedDto>>
{
    private const string Fido2AttestationOptions = nameof(Fido2AttestationOptions);

    public async Task<IResult<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var attestationOptionsForUser = Encoding.UTF8.GetString(
            await distributedCache.GetAsync($"{context.ConfirmRequest.Email}{Fido2AttestationOptions}", token) ?? []);
        if (string.IsNullOrEmpty(attestationOptionsForUser))
        {
            return CreateUserForReaderAccountError<ReaderAccountCreatedDto>.Create(
                AttestationConstants.NoAttestationOptionsInSession);
        }

        var response = await client.GetResponse<DataResult<UserCreationResult>>(context.ConfirmRequest.ToCreateUser(attestationOptionsForUser!), token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response!.Message.Value;
            var readerAccountUserCreated = new ReaderAccountUserCreated(responseObj!.Id, responseObj.Email);

            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountUserResult,
                Result.Success(readerAccountUserCreated));

            await publishEndpoint.Publish(readerAccountUserCreated, token);

            return await next(context, token);
        }
        else
        {
            return CreateUserForReaderAccountError<ReaderAccountCreatedDto>.Create(
                ReaderAccountUserConstants.UserForReaderAccountUnableToCreateUpdateError, response.Message.Errors);
        }
    }
}