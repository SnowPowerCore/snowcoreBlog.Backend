using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccess.Snapshot;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Delete;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Update;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Delete;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Update;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions.Startup;

public static class CoreServicesConfigurationExtensions
{
    public static IServiceCollection AddCoreServicesConfiguration(this IServiceCollection services)
    {
        // Snapshot pipeline steps
        services.AddScoped<LoadApiAccessSnapshotDataStep>();
        services.AddScoped<BuildApiAccessSnapshotStep>();

        // API access rules pipeline steps (update / delete; create is a direct endpoint)
        services.AddScoped<ValidateApiAccessRuleIdMatchesStep>();
        services.AddScoped<ResolveApiAccessRuleForUpdateStep>();
        services.AddScoped<UpdateApiAccessRuleStep>();
        services.AddScoped<ResolveApiAccessRuleForDeleteStep>();
        services.AddScoped<DeleteApiAccessRuleStep>();

        // API access response templates pipeline steps (update / delete; create is a direct endpoint)
        services.AddScoped<ValidateApiAccessResponseTemplateIdMatchesStep>();
        services.AddScoped<ResolveApiAccessResponseTemplateForUpdateStep>();
        services.AddScoped<UpdateApiAccessResponseTemplateStep>();
        services.AddScoped<ResolveApiAccessResponseTemplateForDeleteStep>();
        services.AddScoped<DeleteApiAccessResponseTemplateStep>();

        return services;
    }
}
