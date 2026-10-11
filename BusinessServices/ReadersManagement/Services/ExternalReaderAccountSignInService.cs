using MassTransit;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Services;

/// <summary>
/// Completes a sign-in initiated by an external identity provider
/// (Microsoft / Google / Apple): resolves the reader account behind the
/// provider-verified email address and issues the JWT pair for it.
/// Deliberately does NOT create accounts — unknown emails are rejected so the
/// reader has to register first.
/// </summary>
public sealed class ExternalReaderAccountSignInService(IRequestClient<ValidateUserExists> requestClient,
                                                      ReaderAccountTokenPairIssuer tokenPairIssuer,
                                                      ILogger<ExternalReaderAccountSignInService> logger)
{
    public async Task<bool> TrySignInAsync(string email, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<UserExistsValidationResult>>(
            new ValidateUserExists { Email = email }, token);
        if (!result.Message.IsSuccess)
        {
            logger.LogWarning("External sign-in: unable to validate account existence for {Email}.", email);
            return false;
        }

        var value = result.Message.Value;
        if (value is not { Exists: true } || value.UserId is not { } userId)
        {
            logger.LogInformation("External sign-in: no reader account exists for {Email}; registration required.", email);
            return false;
        }

        var (success, errors) = await tokenPairIssuer.TryIssueAsync(userId, email, token);
        if (!success)
        {
            logger.LogWarning(
                "External sign-in: token pair issuance failed for {Email}: {Errors}",
                email, string.Join("; ", errors));
        }

        return success;
    }
}
