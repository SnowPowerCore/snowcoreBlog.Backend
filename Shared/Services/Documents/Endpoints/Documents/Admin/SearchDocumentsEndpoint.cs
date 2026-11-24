using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Documents.Core.Entities.Documents;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Documents.Endpoints.Documents.Admin;

/// <summary>
/// Admin: paged search over stored documents (file name contains, optional
/// category and status filters).
/// </summary>
public class SearchDocumentsEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("admin/documents");
        Version(1);
        SerializerContext(DocumentsSerializationContext.Default);
        AllowAnonymous(); // TODO: restrict to admin users once IAM admin accounts exist
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<List<DocumentAdminDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = Query<string?>("search");
        var category = Query<string?>("category");
        var status = Query<int?>("status");
        var page = Math.Max(1, Query<int?>("page") ?? 1);
        var pageSize = Math.Clamp(Query<int?>("pageSize") ?? 25, 1, 100);

        var query = QuerySession.Query<DocumentEntity>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.FileName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }
        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryTerm = category.Trim();
            query = query.Where(x => x.Category == categoryTerm);
        }
        if (status is not null)
        {
            var statusTerm = (DocumentStatus)status;
            query = query.Where(x => x.Status == statusTerm);
        }

        var items = await query
            .OrderByDescending(x => x.UploadedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = Maybe.Create(items
            .Select(static e => new DocumentAdminDto
            {
                Id = e.Id,
                FileName = e.FileName,
                ContentType = e.ContentType,
                SizeBytes = e.SizeBytes,
                Category = e.Category,
                Status = (DocumentStatusDto)(int)e.Status,
                Checksum = e.Checksum,
                OwnerId = e.OwnerId,
                UploadedAt = e.UploadedAt,
            })
            .ToList());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
