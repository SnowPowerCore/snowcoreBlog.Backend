using FluentValidation;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.Backend.Email.Validation;

namespace snowcoreBlog.Backend.Email.Extensions.Startup;

public static class ValidationConfigurationExtensions
{
    public static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<SendGenericEmail>, GenericEmailValidator>();
        services.AddSingleton<IValidator<SendTemplatedEmail>, TemplatedEmailValidator>();
        services.AddSingleton<IValidator<CheckEmailDomain>, CheckEmailDomainValidator>();

        return services;
    }
}