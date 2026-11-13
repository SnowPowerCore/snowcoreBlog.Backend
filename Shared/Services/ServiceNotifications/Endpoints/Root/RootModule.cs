using FastSharp.Modules.Configuration;
using FastSharp.Modules.Core;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints.Root;

public sealed class RootModule : Module
{
    public RootModule()
    {
        ConfigureModule("/", static options => options
            .WithTags("Root"));

        Include<PingEndpoint>();
    }
}
