using System.Text.RegularExpressions;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Articles.Steps;

public partial class GenerateSlugStep(IArticleRepository repo) : IStep<CreateArticleDelegate, CreateArticleContext, IMaybe<CreateArticleResultDto>>
{
    private static readonly Regex _invalid = MyRegex();

    private static string ToSlug(string input)
    {
        var lowered = input.ToLowerInvariant();
        var dashed = MyRegex1().Replace(lowered, "-");
        dashed = _invalid.Replace(dashed, "");
        dashed = MyRegex2().Replace(dashed, "-");
        return dashed.Trim('-');
    }

    public async Task<IMaybe<CreateArticleResultDto>> InvokeAsync(CreateArticleContext context, CreateArticleDelegate next, CancellationToken token = default)
    {
        var baseSlug = ToSlug(context.CreateRequest.Title);
        var slug = baseSlug;
        var suffix = 1;
        while (await repo.SlugExistsAsync(slug, token))
        {
            slug = baseSlug + "-" + suffix++;
        }

        // The save currently happens in this step (scaffold); SaveArticleStep is a pass-through.
        // Create snapshot entity for the markdown
        var snapshot = new Core.Entities.Article.ArticleSnapshotEntity
        {
            Id = Guid.NewGuid(),
            ArticleId = Guid.Empty, // will set after creating Article entity
            Markdown = context.CreateRequest.Markdown,
            Excerpt = context.CreateRequest.Excerpt,
            MetaDescription = context.CreateRequest.MetaDescription,
            ModifiedAt = DateTime.UtcNow
        };

        var articleId = Guid.NewGuid();
        var entity = new Core.Entities.Article.ArticleEntity
        {
            Id = articleId,
            AuthorUserIds = context.ResolvedAuthorUserIds,
            Title = context.CreateRequest.Title,
            Slug = slug,
            LatestSnapshotId = snapshot.Id,
            PublishedAt = null,
            Tags = context.CreateRequest.Tags ?? Array.Empty<string>(),
            CoverImageUrl = context.CreateRequest.CoverImageUrl,
            Category = context.CreateRequest.Category
        };

        // now set snapshot.ArticleId to the articleId
        snapshot = snapshot with { ArticleId = articleId };

        var id = await repo.InsertArticleWithSnapshotAsync(entity, snapshot, token);

        return Maybe.Create(new CreateArticleResultDto { Id = id, Slug = slug });
    }

    [GeneratedRegex("[^a-z0-9-]", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
    [GeneratedRegex("\\s+")]
    private static partial Regex MyRegex1();
    [GeneratedRegex("-+")]
    private static partial Regex MyRegex2();
}