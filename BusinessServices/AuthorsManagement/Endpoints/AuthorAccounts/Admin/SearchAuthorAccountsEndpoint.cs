using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AuthorsManagement.Core.Entities.Author;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AuthorsManagement.Endpoints.AuthorAccounts.Admin;

/// <summary>
/// Admin: paged search over author accounts (display name contains, case-insensitive).
/// </summary>
public class SearchAuthorAccountsEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("admin/accounts");
        Version(1);
        SerializerContext(AuthorsManagementSerializationContext.Default);
        AllowAnonymous(); // TODO: restrict to admin users once IAM admin accounts exist
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<List<AuthorAccountAdminDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = Query<string?>("search");
        var page = Math.Max(1, Query<int?>("page") ?? 1);
        var pageSize = Math.Clamp(Query<int?>("pageSize") ?? 25, 1, 100);

        var query = QuerySession.Query<AuthorEntity>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.DisplayName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var items = await query
            .OrderBy(x => x.DisplayName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = Maybe.Create(items
            .Select(static e => new AuthorAccountAdminDto { Id = e.Id, UserId = e.UserId, DisplayName = e.DisplayName })
            .ToList());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
