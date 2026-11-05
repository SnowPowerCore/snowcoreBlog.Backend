using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Admins;

/// <summary>
/// Admin: paged search over admin accounts (email/nickname contains).
/// </summary>
public class SearchAdminAccountsEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("admins");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<List<AdminDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var search = Query<string?>("search");
        var page = Math.Max(1, Query<int?>("page") ?? 1);
        var pageSize = Math.Clamp(Query<int?>("pageSize") ?? 25, 1, 100);

        var query = QuerySession.Query<AdminEntity>().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(x =>
                x.Email.Contains(term, StringComparison.OrdinalIgnoreCase)
                || x.NickName.Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        var items = await query
            .OrderBy(static x => x.Email)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = Maybe.Create(items.Select(static a => a.ToDto()).ToList());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
