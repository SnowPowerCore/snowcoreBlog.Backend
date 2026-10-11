using snowcoreBlog.Backend.Core.Entities.UrlShortener;

namespace snowcoreBlog.Backend.UrlShortener.Context;

public class ResolveShortUrlContext
{
    public string Code { get; }

    // Filled by steps
    public UrlMappingEntity? Mapping { get; set; }

    public ResolveShortUrlContext(string code)
    {
        Code = code ?? string.Empty;
    }
}
