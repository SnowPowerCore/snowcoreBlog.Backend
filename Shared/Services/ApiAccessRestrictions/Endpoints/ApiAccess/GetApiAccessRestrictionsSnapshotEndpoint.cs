using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccess.Snapshot;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccess;

/// <summary>
/// Public, unauthenticated snapshot of all access restrictions (consumed by the
/// gateway's background refresher). Loads the four restriction repositories and
/// compiles the wire snapshot through the stepified pipeline.
/// </summary>
public class GetApiAccessRestrictionsSnapshotEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(LoadApiAccessSnapshotDataStep),
        typeof(BuildApiAccessSnapshotStep),
    ])]
    protected GetApiAccessRestrictionsSnapshotDelegate GetApiAccessRestrictionsSnapshot { get; } = default!;

    public override void Configure()
    {
        Get("snapshot");
        AllowAnonymous();
        Version(1);
        SerializerContext(ApiAccessRestrictionsSerializationContext.Default);
        Description(b => b
            .WithTags("ApiAccess")
            .Produces<ApiResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.BadRequest, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await GetApiAccessRestrictionsSnapshot(new GetApiAccessRestrictionsSnapshotContext(), ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
