using System.Net;
using System.Net.Mime;
using FastEndpoints;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.Articles.Endpoints.Root;

public class PingEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("ping");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Produces<string>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override Task HandleAsync(CancellationToken ct) =>
        SendAsync("pong", (int)HttpStatusCode.OK, ct);
}
