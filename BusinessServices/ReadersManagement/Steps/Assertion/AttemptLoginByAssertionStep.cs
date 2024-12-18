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
using snowcoreBlog.Backend.ReadersManagement.ErrorResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class AttemptLoginByAssertionStep(IRequestClient<LoginUser> requestClient,
                                         IPublishEndpoint publishEndpoint,
                                         IConnectionMultiplexer redis) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IResult<LoginByAssertionResultDto>>
{
    private const string Fido2AssertionOptions = nameof(Fido2AssertionOptions);

    public async Task<IResult<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var db = redis.GetDatabase();
        var redisValue = await db.StringGetAsync($"{context.LoginByAssertion.Email}{Fido2AssertionOptions}");
        var assertionOptionsForUser = Encoding.UTF8.GetString(redisValue != RedisValue.Null ? redisValue! : []);
        if (string.IsNullOrEmpty(assertionOptionsForUser))
        {
            return AssertionError<LoginByAssertionResultDto>.Create(
                AssertionConstants.NoAssertionOptionsInSession);
        }

        var result = await requestClient.GetResponse<DataResult<UserLoginResult>>(
            context.LoginByAssertion.ToLoginUser(assertionOptionsForUser));
        if (result.Message.IsSuccess)
        {
            await publishEndpoint.Publish<ReaderAccountUserLoggedIn>(
                new(result.Message.Value!.Id, context.LoginByAssertion.Email), token);

            return Result.Success<LoginByAssertionResultDto>(new());
        }
        else
        {
            return UserLoginError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, result.Message.Errors);
        }
    }
}