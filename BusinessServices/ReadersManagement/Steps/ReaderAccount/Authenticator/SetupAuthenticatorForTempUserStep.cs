using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using QRCoder;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Constants;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;

public class SetupAuthenticatorForTempUserStep(IRequestClient<SetupUserAuthenticator> requestClient) : IStep<SetupAuthenticatorDelegate, SetupAuthenticatorContext, IMaybe<SetupReaderAccountAuthenticatorResultDto>>
{
    public async Task<IMaybe<SetupReaderAccountAuthenticatorResultDto>> InvokeAsync(SetupAuthenticatorContext context, SetupAuthenticatorDelegate next, CancellationToken token = default)
    {
        var result = await requestClient.GetResponse<DataResult<AuthenticatorSetupResult>>(
            new SetupUserAuthenticator
            {
                Email = context.SetupRequest.Email,
                TempUserVerificationToken = context.SetupRequest.VerificationToken
            }, token);
        if (result.Message.IsSuccess)
        {
            var setup = result.Message.Value!;

            return Maybe.Create(new SetupReaderAccountAuthenticatorResultDto(
                setup.Secret,
                setup.OtpAuthUri,
                BuildQrCodeSvg(setup.OtpAuthUri)));
        }
        else
        {
            return CreateUserForReaderAccountError<SetupReaderAccountAuthenticatorResultDto>.Create(
                ReaderAccountUserConstants.TempUserForReaderAccountUnableToCreateUpdateError, result.Message.Errors);
        }
    }

    private static string BuildQrCodeSvg(string otpAuthUri)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(otpAuthUri, QRCodeGenerator.ECCLevel.M);
        return new SvgQRCode(data).GetGraphic(pixelsPerModule: 6);
    }
}
