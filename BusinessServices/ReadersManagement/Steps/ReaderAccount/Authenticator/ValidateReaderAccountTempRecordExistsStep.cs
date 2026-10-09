using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;

public class ValidateReaderAccountTempRecordExistsStep(IRequestClient<ValidateTempUserExists> requestClient) : IStep<SetupAuthenticatorDelegate, SetupAuthenticatorContext, IMaybe<SetupReaderAccountAuthenticatorResultDto>>
{
    public async Task<IMaybe<SetupReaderAccountAuthenticatorResultDto>> InvokeAsync(SetupAuthenticatorContext context, SetupAuthenticatorDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<TempUserExistsValidationResult>>(
            new ValidateTempUserExists { Email = context.SetupRequest.Email }, token);
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return await next(context, token);
            }
            else
            {
                return ReaderAccountNotExistsError<SetupReaderAccountAuthenticatorResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountNotExistsError);
            }
        }
        else
        {
            return ReaderAccountNotExistsError<SetupReaderAccountAuthenticatorResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError, result.Message.Errors);
        }
    }
}
