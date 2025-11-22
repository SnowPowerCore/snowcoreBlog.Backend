using FastEndpoints;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Root;

public class RootEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(string.Empty);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Options(x => x.ExcludeFromDescription());
    }

    public override Task HandleAsync(CancellationToken ct) =>
        Send.RedirectAsync("~/scalar/v1");
}