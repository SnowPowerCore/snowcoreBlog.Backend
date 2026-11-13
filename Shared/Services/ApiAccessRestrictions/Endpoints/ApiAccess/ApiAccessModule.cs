using FastSharp.Modules.Configuration;
using FastSharp.Modules.Core;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccess;

/// <summary>
/// Public, unauthenticated surface used by the gateway middleware (check + snapshot refresh).
/// </summary>
public sealed class ApiAccessModule : Module
{
    public ApiAccessModule()
    {
        ConfigureModule("/", static options => options
            .WithTags("ApiAccess"));

        Include<CheckApiAccessEndpoint>();
        Include<GetApiAccessRestrictionsSnapshotEndpoint>();
    }
}
