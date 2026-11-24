using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Articles.Core.Entities.Article;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles.Admin;

/// <summary>
/// Admin: publishes or unpublishes an article by setting/clearing PublishedAt.
/// </summary>
public class SetArticlePublicationEndpoint : Endpoint<SetArticlePublicationDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public override void Configure()
    {
        Put("admin/articles/{id:guid}/publication");
        Version(1);
        SerializerContext(ArticlesSerializationContext.Default);
        AllowAnonymous(); // TODO: restrict to admin users once IAM admin accounts exist
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<ArticleAdminDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(SetArticlePublicationDto req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var article = await DocumentSession.LoadAsync<ArticleEntity>(id, ct);

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

        var publishedAt = req.Publish
            ? (req.PublishedAt?.UtcDateTime ?? DateTime.UtcNow)
            : (DateTime?)null;

        // ArticleEntity is an init-only record вЂ” update by storing a `with` copy (same Id = upsert).
        var updated = article with { PublishedAt = publishedAt };
        DocumentSession.Store(updated);
        await DocumentSession.SaveChangesAsync(ct);

        var result = Maybe.Create(new ArticleAdminDto
        {
            Id = updated.Id,
            Title = updated.Title,
            Slug = updated.Slug,
            Tags = updated.Tags,
            Category = updated.Category,
            CoverImageUrl = updated.CoverImageUrl,
            LatestSnapshotId = updated.LatestSnapshotId,
            PublishedAt = updated.PublishedAt,
        });

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
