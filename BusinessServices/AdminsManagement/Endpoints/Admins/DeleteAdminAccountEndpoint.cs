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
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Admins;

/// <summary>
/// Admin: soft-deletes an admin account. Guards: an admin cannot delete
/// themselves and the LAST active admin can never be deleted.
/// </summary>
public class DeleteAdminAccountEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public override void Configure()
    {
        Delete("admins/{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Produces<ApiResponseForOpenApi<AdminDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.Conflict, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var currentAdminId = User.GetAdminId();

        if (id == currentAdminId)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["You cannot delete your own admin account."],
                    (int)HttpStatusCode.Conflict),
                (int)HttpStatusCode.Conflict,
                ct);
            return;
        }

        var entity = await DocumentSession.LoadAsync<AdminEntity>(id, ct);
        if (entity is default(AdminEntity))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Admin account with ID '{id}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        // Enforce at least one remaining active admin.
        var activeAdmins = await DocumentSession.Query<AdminEntity>()
            .Where(static x => x.IsActive)
            .ToListAsync(ct);
        if (entity.IsActive && !activeAdmins.Any(x => x.Id != id))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["The last active admin cannot be deleted."],
                    (int)HttpStatusCode.Conflict),
                (int)HttpStatusCode.Conflict,
                ct);
            return;
        }

        DocumentSession.Delete(entity);
        await DocumentSession.SaveChangesAsync(ct);

        var result = Maybe.Create(entity.ToDto());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
