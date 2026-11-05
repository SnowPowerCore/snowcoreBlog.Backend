using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Entities.Article;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles.Admin;

/// <summary>
/// Admin: article details incl. the latest content snapshot (markdown).
/// </summary>
public class GetArticleForAdminEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("admin/articles/{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous(); // TODO: restrict to admin users once IAM admin accounts exist
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<ArticleAdminDetailDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var article = await QuerySession.LoadAsync<ArticleEntity>(id, ct);

        if (article is default(ArticleEntity))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Article with ID '{id}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var snapshot = await QuerySession.Query<ArticleSnapshotEntity>()
            .Where(x => x.ArticleId == id)
            .OrderByDescending(x => x.ModifiedAt)
            .FirstOrDefaultAsync(ct);

        var result = Maybe.Create(new ArticleAdminDetailDto
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Tags = article.Tags,
            Category = article.Category,
            CoverImageUrl = article.CoverImageUrl,
            LatestSnapshotId = article.LatestSnapshotId,
            PublishedAt = article.PublishedAt,
            Markdown = snapshot?.Markdown ?? string.Empty,
            Excerpt = snapshot?.Excerpt,
            MetaDescription = snapshot?.MetaDescription,
            SnapshotModifiedAt = snapshot?.ModifiedAt ?? default,
        });

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
