using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Invites;

/// <summary>
/// Admin: lists invites (newest first).
/// </summary>
public class GetAdminInvitesEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IQuerySession QuerySession { get; set; }

    public override void Configure()
    {
        Get("invites");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<List<AdminInviteDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var items = await QuerySession.Query<AdminInviteEntity>()
            .OrderByDescending(static x => x.CreatedAtUtc)
            .Take(100)
            .ToListAsync(ct);

        var result = Maybe.Create(items.Select(static i => new AdminInviteDto
        {
            Id = i.Id,
            Email = i.Email,
            NickName = i.NickName,
            InvitedByAdminId = i.InvitedByAdminId,
            CreatedAtUtc = i.CreatedAtUtc,
            ExpiresAtUtc = i.ExpiresAtUtc,
            AcceptedAtUtc = i.AcceptedAtUtc,
        }).ToList());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
