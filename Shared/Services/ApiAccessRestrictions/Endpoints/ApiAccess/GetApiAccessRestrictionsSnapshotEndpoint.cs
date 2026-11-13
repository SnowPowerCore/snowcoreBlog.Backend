using FastSharp.Modules.Core;
using MaybeResults;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccess.Snapshot;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccess;

public sealed class GetApiAccessRestrictionsSnapshotEndpoint : IEndpoint
{
    [StepifiedProcess(Steps = [
        typeof(LoadApiAccessSnapshotDataStep),
        typeof(BuildApiAccessSnapshotStep),
    ])]
    protected GetApiAccessRestrictionsSnapshotDelegate GetApiAccessRestrictionsSnapshot { get; } = default!;

    public void Map(RouteGroupBuilder app)
    {
        app.MapGet("/snapshot/v1", async (CancellationToken ct) =>
        {
            var result = await GetApiAccessRestrictionsSnapshot(new GetApiAccessRestrictionsSnapshotContext(), ct);

            return result is Some<ApiAccessRestrictionsSnapshotDto> snapshot
                ? Results.Json(snapshot.Value)
                : EndpointResults.Errors(StatusCodes.Status400BadRequest);
        });
    }
}
