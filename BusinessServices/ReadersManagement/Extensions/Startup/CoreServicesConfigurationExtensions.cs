using Ixnas.AltchaNet;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Infrastructure.Middleware;
using snowcoreBlog.Backend.Infrastructure.Services;
using snowcoreBlog.Backend.Infrastructure.Stores;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;
using snowcoreBlog.Backend.ReadersManagement.Steps.Assertion;
using snowcoreBlog.Backend.ReadersManagement.Steps.Attestation;
using snowcoreBlog.Backend.ReadersManagement.Steps.NickName;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm;
using snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request;
using System.IdentityModel.Tokens.Jwt;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IHasher, Argon2Hasher>();
        services.AddScoped<JwtSecurityTokenHandler>();
        services.AddScoped<UserCookieJsonWebTokenMiddleware>();

        // Steps
        services.AddScoped<ValidateNickNameWasNotTakenStep>();
        services.AddScoped<ValidateReaderAccountTempRecordNotExistsStep>();
        services.AddScoped<ValidateReaderAccountExistsStep>();
        services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Request.ValidateReaderAccountNotExistsStep>();
        services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount.Confirm.ValidateReaderAccountNotExistsStep>();
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
        services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.Tokens.ResolveRefreshTokenStep>();
        services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.Tokens.UseRefreshTokenLockStep>();
        services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.Tokens.ValidateRefreshTokenRecordStep>();
        services.AddScoped<snowcoreBlog.Backend.ReadersManagement.Steps.Tokens.RotateReaderTokenPairStep>();

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