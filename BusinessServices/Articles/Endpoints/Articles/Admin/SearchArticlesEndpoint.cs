using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Articles.Core.Entities.Article;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles.Admin;

/// <summary>
/// Admin: paged search over articles (title/slug contains, optional tag filter).
/// </summary>
public class SearchArticlesEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("admin/articles");
        Version(1);
        SerializerContext(ArticlesSerializationContext.Default);
        AllowAnonymous(); // TODO: restrict to admin users once IAM admin accounts exist
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<List<ArticleAdminDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = Query<string?>("search");
        var tag = Query<string?>("tag");
        var page = Math.Max(1, Query<int?>("page") ?? 1);
        var pageSize = Math.Clamp(Query<int?>("pageSize") ?? 25, 1, 100);

        var query = QuerySession.Query<ArticleEntity>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                || x.Slug.ToLower().Contains(term.ToLower()));
        }
        if (!string.IsNullOrWhiteSpace(tag))
        {
            var tagTerm = tag.Trim();
            query = query.Where(x => x.Tags.Contains(tagTerm));
        }

        var items = await query
            .OrderBy(x => x.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = Maybe.Create(items
            .Select(static e => new ArticleAdminDto
            {
                Id = e.Id,
                Title = e.Title,
                Slug = e.Slug,
                Tags = e.Tags,
                Category = e.Category,
                CoverImageUrl = e.CoverImageUrl,
                LatestSnapshotId = e.LatestSnapshotId,
                PublishedAt = e.PublishedAt,
            })
            .ToList());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
