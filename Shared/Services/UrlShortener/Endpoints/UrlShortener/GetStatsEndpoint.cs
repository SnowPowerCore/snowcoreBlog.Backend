using System.Net;
using FastEndpoints;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;

public class GetStatsEndpoint : EndpointWithoutRequest
{
    private readonly IUrlMappingRepository _repo;

    public GetStatsEndpoint(IUrlMappingRepository repo)
    {
        _repo = repo;
    }

    public override void Configure()
    {
        Get("admin/{code}/stats");
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var code = HttpContext.Request.RouteValues["code"]?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(code))
        {
            await Send.StatusCodeAsync((int)HttpStatusCode.BadRequest, ct);
            return;
        }

        var mapping = await _repo.GetByCodeAsync(code, ct);
        if (mapping is null)
        {
            await Send.StatusCodeAsync((int)HttpStatusCode.NotFound, ct);
            return;
        }

        var since = mapping.WindowDurationSeconds.HasValue ? DateTime.UtcNow.AddSeconds(-mapping.WindowDurationSeconds.Value) : DateTime.UtcNow.AddYears(-1);
        var count = await _repo.CountClicksInWindowAsync(mapping.Id, since, ct);

        await Send.ResponseAsync(new { mapping.Code, mapping.OriginalUrl, mapping.CreatedAt, mapping.MaxClicksPerWindow, mapping.WindowDurationSeconds, ClicksInWindow = count }, (int)HttpStatusCode.OK, ct);
    }
}
