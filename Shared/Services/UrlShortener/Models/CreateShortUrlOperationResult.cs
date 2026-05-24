using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.UrlShortener.Models;

public sealed class CreateShortUrlOperationResult
{
    public int HttpStatusCode { get; set; }
    public ApiResponse? Response { get; set; }
}
