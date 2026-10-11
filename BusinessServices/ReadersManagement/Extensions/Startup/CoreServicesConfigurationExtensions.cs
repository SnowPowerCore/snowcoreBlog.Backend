using Ixnas.AltchaNet;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Stores;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;
using snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;
using snowcoreBlog.Backend.ReadersManagement.Steps.NickName;
using snowcoreBlog.Backend.ReadersManagement.Services;
using snowcoreBlog.Backend.ReadersManagement.Steps.OneTimeCode;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Authenticator;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;
using Microsoft.IdentityModel.JsonWebTokens;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;
using snowcoreBlog.Backend.Gateway.Middleware.Middleware;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IHasher, Argon2Hasher>();
        services.AddScoped<JsonWebTokenHandler>();
        services.AddScoped<UserCookieJsonWebTokenMiddleware>();

        // Steps
        services.AddScoped<ValidateNickNameWasNotTakenStep>();
        services.AddScoped<ValidateReaderAccountTempRecordNotExistsStep>();
        services.AddScoped<ValidateReaderAccountExistsStep>();
        services.AddScoped<Steps.ReaderAccount.Request.ValidateReaderAccountNotExistsStep>();
        services.AddScoped<Steps.ReaderAccount.Confirm.ValidateReaderAccountNotExistsStep>();
        services.AddScoped<ValidateReaderAccountEmailDomainStep>();
        services.AddScoped<ValidateReaderAccountNickNameWasNotTakenStep>();
        services.AddScoped<CreateReaderAccountTempUserStep>();
        services.AddScoped<CreateReaderAccountUserStep>();
        services.AddScoped<CreateReaderEntityForNewUserStep>();
        services.AddScoped<ReturnCreatedReaderEntityStep>();
        services.AddScoped<RequestNewAttestationOptionsStep>();
        services.AddScoped<RequestNewAssertionOptionsStep>();
        services.AddScoped<AttemptLoginByAssertionStep>();
        services.AddScoped<GetTokenForReaderAccountStep>();

        // One-time code login (authenticator apps)
        services.AddScoped<ResolveReaderAccountForOneTimeCodeStep>();
        services.AddScoped<VerifyAuthenticatorOneTimeCodeStep>();
        services.AddScoped<IssueTokenForOneTimeCodeLoginStep>();

        // Authenticator (TOTP) registration path
        services.AddScoped<ValidateReaderAccountTempRecordExistsStep>();
        services.AddScoped<SetupAuthenticatorForTempUserStep>();
        services.AddScoped<ValidateReaderAccountNotExistsForAuthenticatorStep>();
        services.AddScoped<CreateReaderAccountUserWithAuthenticatorStep>();
        services.AddScoped<CreateReaderEntityForAuthenticatorStep>();
        services.AddScoped<ReturnCreatedReaderEntityForAuthenticatorStep>();

        // Shared JWT pair issuance (passkey + one-time code + external providers)
        services.AddScoped<ReaderAccountTokenPairIssuer>();
        services.AddScoped<ExternalReaderAccountSignInService>();
        services.AddScoped<Steps.Tokens.ResolveRefreshTokenStep>();
        services.AddScoped<Steps.Tokens.UseRefreshTokenLockStep>();
        services.AddScoped<Steps.Tokens.ValidateRefreshTokenRecordStep>();
        services.AddScoped<Steps.Tokens.RotateReaderTokenPairStep>();

        return services;
    }

    public static IServiceCollection AddReaderRepositoriesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IReaderRepository, ReaderRepository>();

        return services;
    }

    public static IServiceCollection AddAltchaServicesConfiguration(this IServiceCollection services)
    {
        services.AddScoped<IAltchaCancellableChallengeStore, AltchaChallengeStore>();

        return services;
    }
}