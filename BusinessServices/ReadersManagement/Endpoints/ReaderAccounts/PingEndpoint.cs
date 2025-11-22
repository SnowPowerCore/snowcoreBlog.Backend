using System.Net;
using System.Net.Mime;
using FastEndpoints;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts;

public class PingEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("ping");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        Claims("readerAccount");
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
            .Produces<string>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override Task HandleAsync(CancellationToken ct) =>
        Send.ResponseAsync("pong", (int)HttpStatusCode.OK, ct);
}