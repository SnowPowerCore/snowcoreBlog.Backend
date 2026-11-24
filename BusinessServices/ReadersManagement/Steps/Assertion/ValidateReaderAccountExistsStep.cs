using Fido2NetLib;
using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;

public class ValidateReaderAccountExistsStep(IRequestClient<ValidateUserExists> requestClient) : IStep<LoginByAssertionDelegate, LoginByAssertionContext, IMaybe<LoginByAssertionResultDto>>
{
    public async Task<IMaybe<LoginByAssertionResultDto>> InvokeAsync(LoginByAssertionContext context, LoginByAssertionDelegate next, CancellationToken token = default)
    {
        // Passkey sign-ins resolve identity from the assertion's credential owner in IAM —
        // the entered email is advisory and may belong to a different (or no) account.
        if (context.LoginByAssertion.AuthenticatorAssertion is not default(AuthenticatorAssertionRawResponse))
        {
            return await next(context, token);
        }

        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(
            context.LoginByAssertion.ToValidateUserExists(), token);
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return await next(context, token);
            }
            else
            {
                return ReaderAccountNotExistsError<LoginByAssertionResultDto>.Create(
                    ReaderAccountConstants.ReaderAccountNotExistsError);
            }
        }
        else
        {
            return ReaderAccountNotExistsError<LoginByAssertionResultDto>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError, result.Message.Errors);
        }
    }
}