using Fido2NetLib;
using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Extensions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;

public class RequestNewAttestationOptionsStep(IRequestClient<ValidateAndCreateAttestationOnRegister> requestClientOnRegister) : IStep<RequestAttestationOptionsDelegate, RequestAttestationOptionsContext, IResult<CredentialCreateOptions>>
{
    public async Task<IResult<CredentialCreateOptions>> InvokeAsync(RequestAttestationOptionsContext context, RequestAttestationOptionsDelegate next, CancellationToken token = default)
    {
        if (context.RequestAttestationOptionsForRegistration is not default(RequestAttestationOptionsForRegistrationDto))
        {
            var result = await requestClientOnRegister.GetResponse<DataResult<CredentialCreateOptions>>(
                context.RequestAttestationOptionsForRegistration.ToValidateOnRegister(), token);
            if (result.Message.IsSuccess)
            {
                return Result.Success(result.Message.Value)!;
            }
            else
            {
                return AttestationError<CredentialCreateOptions>.Create(
                    AttestationConstants.FailedForRegistration);
            }
        }

        // if (context.RequestAttestationOptionsForLogin is not default(RequestAttestationOptionsForLoginDto))
        // {

        // }

        return AttestationError<CredentialCreateOptions>.Create(AttestationConstants.Failed);
    }
}