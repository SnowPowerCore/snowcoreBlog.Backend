using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Entities.UrlShortener;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Context;

public class CreateShortUrlContext(CreateShortUrlRequest request) : BaseGenericContext
{
    public CreateShortUrlRequest Request { get; } = request;

    public string? Code { get; set; }

    public UrlMappingEntity? Mapping { get; set; }
}
