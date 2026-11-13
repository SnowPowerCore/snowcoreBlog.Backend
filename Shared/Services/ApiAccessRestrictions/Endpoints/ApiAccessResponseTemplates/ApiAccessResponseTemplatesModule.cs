using FastSharp.Modules.Configuration;
using FastSharp.Modules.Core;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public sealed class ApiAccessResponseTemplatesModule : Module
{
    public ApiAccessResponseTemplatesModule()
    {
        ConfigureModule("/templates", static options => options
            .WithTags("ApiAccessResponseTemplates")
            .RequireAuthorization(static policy => policy.RequireClaim(AdminClaimConstants.AdminAccountClaimKey)));

        Include<CreateApiAccessResponseTemplateEndpoint>();
        Include<GetApiAccessResponseTemplatesEndpoint>();
        Include<UpdateApiAccessResponseTemplateEndpoint>();
        Include<DeleteApiAccessResponseTemplateEndpoint>();
    }
}
