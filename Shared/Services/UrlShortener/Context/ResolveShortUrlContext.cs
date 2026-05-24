namespace snowcoreBlog.Backend.UrlShortener.Context;

using snowcoreBlog.Backend.UrlShortener.Core.Entities;

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
