using System.Text;
using Fido2NetLib;
using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.Utilities.DataResult;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;

public class RequestNewAttestationOptionsStep(IRequestClient<ValidateAndCreateAttestation> requestClientOnRegister,
                                              IConnectionMultiplexer redis) : IStep<RequestAttestationOptionsDelegate, RequestAttestationOptionsContext, IResult<CredentialCreateOptions>>
{
    private const string Fido2AttestationOptions = nameof(Fido2AttestationOptions);

    public async Task<IResult<CredentialCreateOptions>> InvokeAsync(RequestAttestationOptionsContext context, RequestAttestationOptionsDelegate next, CancellationToken token = default)
    {
        var result = await requestClientOnRegister.GetResponse<DataResult<CredentialCreateOptions>>(
            context.RequestAttestationOptions.ToValidate(), token);
        if (result.Message.IsSuccess)
        {
            var db = redis.GetDatabase();
            await db.StringSetAsync(
                $"{context.RequestAttestationOptions.Email}{Fido2AttestationOptions}",
                Encoding.UTF8.GetBytes(result.Message.Value!.ToJson()),
                TimeSpan.FromMinutes(5));

            return Result.Success(result.Message.Value)!;
        }
        else
        {
            return AttestationError<CredentialCreateOptions>.Create(
                AttestationConstants.Failed, result.Message.Errors);
        }
    }
}