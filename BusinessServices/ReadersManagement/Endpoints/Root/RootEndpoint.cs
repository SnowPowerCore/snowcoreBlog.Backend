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
    }

    public override Task HandleAsync(CancellationToken ct) =>
        SendRedirectAsync("~/scalar/v1");
}