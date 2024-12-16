using System.Text;
using Fido2NetLib;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class RequestNewAssertionOptionsStep(IRequestClient<ValidateAndCreateAssertion> requestClientOnRegister,
                                            IDistributedCache distributedCache) : IStep<RequestAssertionOptionsDelegate, RequestAssertionOptionsContext, IResult<AssertionOptions>>
{
    private const string Fido2AssertionOptions = nameof(Fido2AssertionOptions);

    public async Task<IResult<AssertionOptions>> InvokeAsync(RequestAssertionOptionsContext context, RequestAssertionOptionsDelegate next, CancellationToken token = default)
    {
        var result = await requestClientOnRegister.GetResponse<DataResult<AssertionOptions>>(
            context.RequestAssertionOptions.ToValidate(), token);
        if (result.Message.IsSuccess)
        {
            await distributedCache.SetAsync(
                $"{context.RequestAssertionOptions.Email}{Fido2AssertionOptions}",
                Encoding.UTF8.GetBytes(result.Message.Value!.ToJson()),
                new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) },
                token);

            return Result.Success(result.Message.Value)!;
        }
        else
        {
            return AssertionError<AssertionOptions>.Create(AssertionConstants.Failed);
        }
    }
}