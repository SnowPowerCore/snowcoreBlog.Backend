using FluentValidation;
using snowcoreBlog.Backend.Push.Validation;
using snowcoreBlog.Backend.Push.Core.Contracts;

namespace snowcoreBlog.Backend.Push.Extensions.Startup;

public static class ValidationConfigurationExtensions
{
    public static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<SendGenericPush>, GenericPushValidator>();

        return services;
    }
}