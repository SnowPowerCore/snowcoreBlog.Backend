using System.Net;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener;
using snowcoreBlog.Backend.UrlShortener.ErrorResults;
using snowcoreBlog.Backend.UrlShortener.Extensions;
using snowcoreBlog.Backend.UrlShortener.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;

/// <summary>
/// Admin: returns click statistics for a short URL. Short and stable by
/// design, so it consumes the repository directly (non-stepified).
/// </summary>
public class GetStatsEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    protected IUrlMappingRepository UrlMappingRepository { get; } = default!;

    public override void Configure()
    {
        Get("admin/{code}/stats");
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var code = Route<string>("code") ?? string.Empty;
        IMaybe<ShortUrlStatsDto> result;

        var mapping = await UrlMappingRepository.GetByCodeAsync(code, ct);
        if (mapping is default(UrlMappingEntity))
        {
            result = ShortUrlNotFoundError<ShortUrlStatsDto>.Create(
                $"Short URL '{ code }' was not found.");
        }
        else
        {
            var since = mapping.WindowDurationSeconds.HasValue
                ? DateTime.UtcNow.AddSeconds(-mapping.WindowDurationSeconds.Value)
                : DateTime.UtcNow.AddYears(-1);

            var dto = mapping.ToStatsDto();
            dto.ClicksInWindow = await UrlMappingRepository.CountClicksInWindowAsync(mapping.Id, since, ct);

            result = Maybe.Create(dto);
        }

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
