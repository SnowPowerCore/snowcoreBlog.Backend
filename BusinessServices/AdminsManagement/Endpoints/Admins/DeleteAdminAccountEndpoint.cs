using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.AdminsManagement.Constants;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Extensions;
using snowcoreBlog.Backend.AdminsManagement.Steps.Admins.Delete;
using snowcoreBlog.Backend.Infrastructure;
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

    [StepifiedProcess(Steps = [
        typeof(ValidateNotDeletingSelfStep),
        typeof(ValidateAdminExistsForDeleteStep),
        typeof(ValidateNotLastActiveAdminStep),
        typeof(SoftDeleteAdminStep),
    ])]
    protected DeleteAdminAccountDelegate DeleteAdminAccount { get; } = default!;

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
        var context = new DeleteAdminAccountContext(Route<Guid>("id"), User.GetAdminId());

        var result = await DeleteAdminAccount(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
