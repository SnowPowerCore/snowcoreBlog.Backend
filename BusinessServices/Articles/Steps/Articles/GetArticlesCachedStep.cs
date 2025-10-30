using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;
using System.Text.Json;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Articles.Context;

namespace snowcoreBlog.Backend.Articles.Steps.Articles;

public class GetArticlesCachedStep(IArticleRepository articleRepository, IConnectionMultiplexer redis) : IStep<GetArticlesCachedDelegate, GetArticlesCachedContext, IMaybe<List<ArticleDto>>>
{
    private const string CacheKey = "articles:latest:10";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(30);

    public async Task<IMaybe<List<ArticleDto>>> InvokeAsync(GetArticlesCachedContext context, GetArticlesCachedDelegate next, CancellationToken token = default)
    {
        var db = redis.GetDatabase();

        var cached = await db.StringGetAsync(CacheKey);
        if (cached.HasValue)
        {
            try
            {
                var fromCache = JsonSerializer.Deserialize(cached.ToString(), CoreSerializationContext.Default.ListArticleDto);
                if (fromCache is not null)
                    return Maybe.Create(fromCache);
            }
            catch { }
        }

        var results = await articleRepository.GetLatestArticlesWithSnapshotsAsync(10, token);

        var dtos = results.Select(r => new ArticleDto
        {
            Id = r.Article.Id,
            Title = r.Article.Title,
            Slug = r.Article.Slug,
            Markdown = r.Snapshot?.Markdown ?? string.Empty,
            AuthorUserIds = r.Article.AuthorUserIds ?? Array.Empty<Guid>(),
            LatestSnapshotId = r.Article.LatestSnapshotId,
            PublishedAt = r.Article.PublishedAt,
            ModifiedAt = r.Snapshot?.ModifiedAt,
            Tags = r.Article.Tags,
            CoverImageUrl = r.Article.CoverImageUrl
        }).ToList();

        // Cache the serialized DTO list for TTL
        try
        {
            var json = JsonSerializer.Serialize(dtos, CoreSerializationContext.Default.ListArticleDto);
            await db.StringSetAsync(CacheKey, json, CacheTtl);
        }
        catch { }

        return Maybe.Create(dtos);
    }
}