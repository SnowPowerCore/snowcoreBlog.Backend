using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Invites;

/// <summary>
/// Admin: revokes (soft-deletes) a pending invite.
/// </summary>
public class RevokeAdminInviteEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public override void Configure()
    {
        Delete("invites/{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var invite = await DocumentSession.LoadAsync<AdminInviteEntity>(id, ct);

        if (invite is default(AdminInviteEntity))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Invite with ID '{id}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        DocumentSession.Delete(invite);
        await DocumentSession.SaveChangesAsync(ct);

        await Send.ResponseAsync(
            new ApiResponse(StatusCode: (int)HttpStatusCode.OK),
            (int)HttpStatusCode.OK,
            ct);
    }
}
