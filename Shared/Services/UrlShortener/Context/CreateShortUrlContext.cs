using snowcoreBlog.Backend.Core.Entities.UrlShortener;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Context;

public class CreateShortUrlContext
{
    public CreateShortUrlRequest Request { get; }

    // Filled by steps
    public string? Code { get; set; }
    public UrlMappingEntity? Mapping { get; set; }

    public CreateShortUrlContext(CreateShortUrlRequest req)
    {
        Request = req;
    }
}
