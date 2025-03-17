using System.Text;
using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class CreateReaderAccountUserStep(IRequestClient<CreateUser> client,
                                         IPublishEndpoint publishEndpoint,
                                         IConnectionMultiplexer redis) : IStep<ConfirmCreateReaderAccountDelegate, ConfirmCreateReaderAccountContext, IMaybe<ReaderAccountCreatedDto>>
{
    private const string Fido2AttestationOptions = nameof(Fido2AttestationOptions);

    public async Task<IMaybe<ReaderAccountCreatedDto>> InvokeAsync(ConfirmCreateReaderAccountContext context, ConfirmCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var db = redis.GetDatabase();
        var redisValue = await db.StringGetAsync($"{context.ConfirmRequest.Email}{Fido2AttestationOptions}");
        var attestationOptionsForUser = Encoding.UTF8.GetString(redisValue != RedisValue.Null ? redisValue! : []);
        if (string.IsNullOrEmpty(attestationOptionsForUser))
        {
            return CreateUserForReaderAccountError<ReaderAccountCreatedDto>.Create(
                AttestationConstants.NoAttestationOptionsInSession);
        }

        var response = await client.GetResponse<DataResult<UserCreationResult>>(
            context.ConfirmRequest.ToCreateUser(attestationOptionsForUser!), token);
        if (response.Message.IsSuccess)
        {
            var responseObj = response!.Message.Value;
            var readerAccountUserCreated = new ReaderAccountUserCreated(responseObj!.Id, responseObj.Email);

            context.SetDataWith(
                ReaderAccountConstants.CreateReaderAccountUserResult,
                Maybe.Create(readerAccountUserCreated));

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