using FluentValidation;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.Articles.Extensions.Startup;

public static class ValidationConfigurationExtensions
{
    public static IServiceCollection AddValidationConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IValidator<CreateArticleDto>, CreateArticleValidator>();

        return services;
    }
}
