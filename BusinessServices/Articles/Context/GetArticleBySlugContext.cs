namespace snowcoreBlog.Backend.Articles.Context;

public class GetArticleBySlugContext(string slug)
{
    public string Slug { get; } = slug;
}
