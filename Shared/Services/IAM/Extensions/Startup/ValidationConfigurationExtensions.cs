using FluentValidation;
using snowcoreBlog.Backend.IAM.Validation;
using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Extensions.Startup;

public static class ValidationConfigurationExtensions
{
    public static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<LoginUser>, LoginUserValidator>();
        services.AddSingleton<IValidator<CreateUser>, CreateUserValidator>();
        services.AddSingleton<IValidator<CreateTempUser>, CreateTempUserValidator>();
        services.AddSingleton<IValidator<SetupUserAuthenticator>, SetupUserAuthenticatorValidator>();
        services.AddSingleton<IValidator<CreateUserWithAuthenticator>, CreateUserWithAuthenticatorValidator>();
        services.AddSingleton<IValidator<ValidateUserOneTimeCode>, ValidateUserOneTimeCodeValidator>();

        return services;
    }
}