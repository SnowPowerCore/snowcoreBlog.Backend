using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Core.Entities.UrlShortener;

namespace snowcoreBlog.Backend.UrlShortener.Context;

public class ResolveShortUrlContext(string code) : BaseGenericContext
{
    public string Code { get; } = code ?? string.Empty;

    public UrlMappingEntity? Mapping { get; set; }
}
