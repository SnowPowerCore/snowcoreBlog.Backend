namespace snowcoreBlog.Backend.UrlShortener.Options;

public record UrlShortenerOptions
{
    public string BaseUrl { get; init; } = "https://localhost";
    public int DefaultMaxClicksPerWindow { get; init; } = 0;
    public int DefaultWindowSeconds { get; init; } = 0;
}
