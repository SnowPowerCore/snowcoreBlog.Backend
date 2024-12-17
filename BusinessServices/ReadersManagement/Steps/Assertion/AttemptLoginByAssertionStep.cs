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

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class AttemptLoginByAssertionStep(IRequestClient<LoginUser> requestClient,
                                         IPublishEndpoint publishEndpoint,
                                         IDistributedCache distributedCache) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IResult<LoginByAssertionResultDto>>
{
    private const string Fido2AssertionOptions = nameof(Fido2AssertionOptions);

    public async Task<IResult<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        var assertionOptionsForUser = Encoding.UTF8.GetString(
            await distributedCache.GetAsync($"{context.LoginByAssertion.Email}{Fido2AssertionOptions}", token) ?? []);
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

            return Result.Success(new LoginByAssertionResultDto());
        }
        else
        {
            return UserLoginError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToLogIn, result.Message.Errors);
        }
    }
}