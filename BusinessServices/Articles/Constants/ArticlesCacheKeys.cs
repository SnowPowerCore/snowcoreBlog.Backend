namespace snowcoreBlog.Backend.Articles.Constants;

/// <summary>
/// Redis cache keys shared between the steps that populate and evict the articles cache.
/// </summary>
public static class ArticlesCacheKeys
{
    /// <summary>Latest articles list shown on the home page.</summary>
    public const string Latest = "articles:latest:10";
}
