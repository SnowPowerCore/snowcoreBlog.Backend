using Microsoft.Extensions.DependencyInjection;

namespace snowcoreBlog.Backend.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection SetJsonSerializationContext(this IServiceCollection services) =>
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolverChain.Insert(0, CoreSerializationContext.Default);
        });
}