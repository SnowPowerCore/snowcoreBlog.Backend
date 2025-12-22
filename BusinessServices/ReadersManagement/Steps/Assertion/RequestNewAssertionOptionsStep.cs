using System.Text;
using Fido2NetLib;
using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class RequestNewAssertionOptionsStep(IRequestClient<ValidateAndCreateAssertion> requestClientOnLogin,
                                            IConnectionMultiplexer redis) : IStep<RequestAssertionOptionsDelegate, RequestAssertionOptionsContext, IMaybe<AssertionOptions>>
{
    private const string Fido2AssertionOptions = nameof(Fido2AssertionOptions);

    public async Task<IMaybe<AssertionOptions>> InvokeAsync(RequestAssertionOptionsContext context, RequestAssertionOptionsDelegate next, CancellationToken token = default)
    {
        var result = await requestClientOnLogin.GetResponse<DataResult<AssertionOptions>>(
            context.RequestAssertionOptions.ToValidate(), token);
        if (result.Message.IsSuccess)
        {
            var db = redis.GetDatabase();
            var json = result.Message.Value!.ToJson();
            await db.StringSetAsync(
                $"{context.RequestAssertionOptions.Email}{Fido2AssertionOptions}",
                json,
                TimeSpan.FromMinutes(5));

            return Maybe.Create(result.Message.Value)!;
        }
        else
        {
            return AssertionError<AssertionOptions>.Create(
                AssertionConstants.Failed, result.Message.Errors);
        }
    }
}