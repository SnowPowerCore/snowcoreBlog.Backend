using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Models;

public sealed class ResolveShortUrlOperationResult
{
    public int HttpStatusCode { get; set; }
    public ApiResponse? Response { get; set; }
    public string? RedirectUrl { get; set; }
}
