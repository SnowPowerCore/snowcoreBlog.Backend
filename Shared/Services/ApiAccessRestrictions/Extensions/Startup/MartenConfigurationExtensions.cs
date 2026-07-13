using Marten;
using Marten.Services;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Services;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class MartenConfigurationExtensions
{
    private static readonly SystemTextJsonSerializer Serializer = new();

    public static IServiceCollection AddMartenConfiguration(this IServiceCollection services)
    {
        services.AddMarten(options =>
        {
            options.RegisterDocumentType<IpRestrictionEntity>();
            options.RegisterDocumentType<RegionRestrictionEntity>();
            options.RegisterDocumentType<ApiAccessRuleEntity>();
            options.RegisterDocumentType<ApiAccessResponseTemplateEntity>();
            Serializer.UseTypeInfoResolver(CoreSerializationContext.Default);
            options.Serializer(Serializer);
            options.Policies.AllDocumentsSoftDeleted();
        })
            .UseLightweightSessions()
            .UseNpgsqlDataSource();

        services.AddScoped<IIpRestrictionRepository, IpRestrictionRepository>();
        services.AddScoped<IRequestRestrictionService, RequestRestrictionService>();
        services.AddScoped<IRegionRestrictionRepository, RegionRestrictionRepository>();
        services.AddScoped<IApiAccessRuleRepository, ApiAccessRuleRepository>();
        services.AddScoped<IApiAccessResponseTemplateRepository, ApiAccessResponseTemplateRepository>();
        services.AddScoped<IApiAccessRestrictionEvaluator, ApiAccessRestrictionEvaluator>();

        return services;
    }
}