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

        // pass slug via modifying the request object? We'll create an ArticleEntity in next step using context
        // For simplicity, create a new context wrapper with slug via extension method pattern not available, so attach via reflection is not needed.
        // We'll set an additional property via dynamic — but simpler: store slug into a new context type is heavier. Instead, we'll create ArticleEntity here and call next with same context (next will read nothing).
        // To keep pipeline simple, attach slug to context by using a small local store on context via a dictionary is overkill. Instead, use a continuation delegate that saves entity here and ends pipeline by returning result.

        // Create entity and persist in repository via next? But repo insertion is in SaveArticleStep. To avoid complex context mutation, we'll set slug in a synthetic property by creating a derived context is complex.
        // Simpler approach: call next and ignore; but we need slug later. To avoid blocking, we will store slug on context via runtime expando using a property bag if needed. But C# record isn't dynamic.

        // For now, store slug in a header-like static? Not acceptable. Easiest: call next which will be SaveArticleStep that expects slug in context.CreateRequest.Title? We'll instead modify the CreateRequest by creating a new CreateArticleDto with slug in Markdown? Not ideal.

        // Create snapshot entity for the markdown
        var snapshot = new Core.Entities.Article.ArticleSnapshotEntity
        {
            Id = Guid.NewGuid(),
            ArticleId = Guid.Empty, // will set after creating Article entity
            Markdown = context.CreateRequest.Markdown,
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
            CoverImageUrl = context.CreateRequest.CoverImageUrl
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