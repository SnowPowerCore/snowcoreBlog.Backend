using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.ReadersManagement.Core.Entities.Reader;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.ReaderAccounts.Admin;

/// <summary>
/// Admin: paged search over reader accounts (nickname contains, case-insensitive).
/// </summary>
public class SearchReaderAccountsEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("admin/accounts");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous(); // TODO: restrict to admin users once IAM admin accounts exist
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<List<ReaderAccountAdminDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = Query<string?>("search");
        var page = Math.Max(1, Query<int?>("page") ?? 1);
        var pageSize = Math.Clamp(Query<int?>("pageSize") ?? 25, 1, 100);

        var query = QuerySession.Query<ReaderEntity>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x => x.NickName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var items = await query
            .OrderBy(x => x.NickName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = Maybe.Create(items
            .Select(static e => new ReaderAccountAdminDto { Id = e.Id, UserId = e.UserId, NickName = e.NickName })
            .ToList());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
