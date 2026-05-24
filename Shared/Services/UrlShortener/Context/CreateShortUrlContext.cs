namespace snowcoreBlog.Backend.UrlShortener.Context;

using snowcoreBlog.Backend.UrlShortener.Endpoints.UrlShortener;
using snowcoreBlog.Backend.UrlShortener.Core.Entities;

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
