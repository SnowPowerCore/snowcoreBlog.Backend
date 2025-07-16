using System.Text;
using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using MaybeResults;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.ErrorResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class AttemptLoginByAssertionStep(IRequestClient<LoginUser> requestClient,
                                         IPublishEndpoint publishEndpoint,
                                         IConnectionMultiplexer redis) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    private const string Fido2AssertionOptions = nameof(Fido2AssertionOptions);

    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var db = redis.GetDatabase();
        var redisValue = await db.StringGetAsync($"{context.LoginByAssertion.Email}{Fido2AssertionOptions}");
        var assertionOptionsForUser = Encoding.UTF8.GetString(redisValue != RedisValue.Null ? redisValue! : []);
        if (string.IsNullOrWhiteSpace(assertionOptionsForUser))
        {
            return AssertionError<LoginByAssertionResultDto>.Create(
                AssertionConstants.NoAssertionOptionsInSession);
        }

        var result = await requestClient.GetResponse<DataResult<UserLoginResult>>(
            context.LoginByAssertion.ToLoginUser(assertionOptionsForUser), token);
        if (result.Message.IsSuccess)
        {
            await publishEndpoint.Publish<ReaderAccountUserLoggedIn>(
                new(result.Message.Value!.Id, context.LoginByAssertion.Email), token);

            context.SetDataWith(ReaderAccountUserConstants.CurrentUserId, result.Message.Value.Id);

            return await next(context, token);
        }
        else
        {
            return UserLoginError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, result.Message.Errors);
        }
    }
}