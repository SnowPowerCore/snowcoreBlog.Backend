using System.Net;
using System.Net.Mime;
using FastEndpoints;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Articles.Endpoints.Root;

public class PingEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("ping");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Description(b => b
            .WithTags(ApiTagConstants.Articles)
            .Produces<string>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override Task HandleAsync(CancellationToken ct) =>
        Send.ResponseAsync("pong", (int)HttpStatusCode.OK, ct);
}
