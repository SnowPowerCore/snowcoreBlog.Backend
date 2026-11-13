using FastSharp.Modules.Core;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccess;

public sealed class CheckApiAccessEndpoint(IApiAccessRestrictionEvaluator evaluator) : IEndpoint
{
    public void Map(RouteGroupBuilder app)
    {
        app.MapPost("/check/v1", async (CheckApiAccessRequestDto request, CancellationToken ct) =>
        {
            var result = await evaluator.EvaluateAsync(request, ct);
            return Results.Json(result);
        });
    }
}
