using System.Text.Json;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.ErrorResults;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.Articles.Steps.Articles;

public class GetArticleBySlugStep(IArticleRepository articleRepository, IConnectionMultiplexer redis) : IStep<GetArticleBySlugDelegate, GetArticleBySlugContext, IMaybe<ArticleDto>>
{
    private const string CacheKeyPrefix = "articles:slug:";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

    public async Task<IMaybe<ArticleDto>> InvokeAsync(GetArticleBySlugContext context, GetArticleBySlugDelegate next, CancellationToken token = default)
    {
        var db = redis.GetDatabase();
        var cacheKey = CacheKeyPrefix + context.Slug;

        var cached = await db.StringGetAsync(cacheKey);
        if (cached.HasValue)
        {
            try
            {
                var fromCache = JsonSerializer.Deserialize(cached.ToString(), CoreSerializationContext.Default.ArticleDto);
                if (fromCache is not default(ArticleDto))
                    return Maybe.Create(fromCache);
            }
            catch { }
        }

        var result = await articleRepository.GetBySlugWithLatestSnapshotAsync(context.Slug, token);
        if (result is not ((var article, var snapshot)))
            return ArticleNotFoundError<ArticleDto>.Create($"Article with slug '{context.Slug}' was not found.");

        var dto = new ArticleDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Markdown = snapshot?.Markdown ?? string.Empty,
            AuthorUserIds = article.AuthorUserIds ?? Array.Empty<Guid>(),
            LatestSnapshotId = article.LatestSnapshotId,
            PublishedAt = article.PublishedAt,
            ModifiedAt = snapshot?.ModifiedAt,
            Tags = article.Tags,
            CoverImageUrl = article.CoverImageUrl,
            Category = article.Category,
            Excerpt = snapshot?.Excerpt,
            MetaDescription = snapshot?.MetaDescription
        };

        // Cache the serialized DTO for TTL
        try
        {
            var json = JsonSerializer.Serialize(dto, CoreSerializationContext.Default.ArticleDto);
            await db.StringSetAsync(cacheKey, json, CacheTtl);
        }
        catch { }

        return Maybe.Create(dto);
    }
}
