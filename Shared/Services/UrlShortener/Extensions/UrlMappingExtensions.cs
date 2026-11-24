using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.UrlShortener.Core.Entities.UrlShortener;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Extensions;

[Mapper]
public static partial class UrlMappingExtensions
{
    public static partial CreateShortUrlResult ToCreateShortUrlResult(this UrlMappingEntity entity);

    [MapProperty(nameof(UrlMappingEntity.OriginalUrl), nameof(ResolveShortUrlResultDto.RedirectUrl))]
    public static partial ResolveShortUrlResultDto ToResolveShortUrlResult(this UrlMappingEntity entity);

    public static partial ShortUrlStatsDto ToStatsDto(this UrlMappingEntity entity);
}
