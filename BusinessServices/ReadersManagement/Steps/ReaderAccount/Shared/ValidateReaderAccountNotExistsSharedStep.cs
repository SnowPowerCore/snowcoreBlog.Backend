using MassTransit;
using MaybeResults;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Shared;

public static class ValidateReaderAccountNotExistsSharedStep
{
    public static async Task<IMaybe<bool>> CheckAsync(IRequestClient<ValidateUserExists> requestClient, ValidateUserExists request, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(request, token);
        if (result.Message.IsSuccess)
        {
            if (result.Message.Value!.Exists)
            {
                return ReaderAccountAlreadyExistsError<bool>.Create(
                    ReaderAccountConstants.ReaderAccountAlreadyExistsError);
            }
            else
            {
                return Maybe.Create(true);
            }
        }
        else
        {
            return CreateUserForReaderAccountError<bool>.Create(
                ReaderAccountConstants.ReaderAccountUnableToCheckIfExistsError, result.Message.Errors);
        }
    }
}