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
using snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Create;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Validation.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Invites;

/// <summary>
/// Admin: invites a new admin. Creates a single-use tokenized invite and
/// emails the acceptance link. Admins CANNOT self-register — this is the
/// only path to a new admin account.
/// </summary>
public class CreateAdminInviteEndpoint : Endpoint<CreateAdminInviteDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateNoAdminWithEmailStep),
        typeof(ValidateNoOpenInviteStep),
        typeof(CreateAdminInviteEntityStep),
        typeof(SendAdminInviteEmailStep),
    ])]
    protected CreateAdminInviteDelegate CreateAdminInvite { get; } = default!;

    public override void Configure()
    {
        Post("invites");
        Version(1);
        SerializerContext(AdminsManagementSerializationContext.Default);
        Validator<CreateAdminInviteValidator>();
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Accepts<CreateAdminInviteDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AdminInviteDto>>((int)HttpStatusCode.Created, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.Conflict, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateAdminInviteDto req, CancellationToken ct)
    {
        var context = new CreateAdminInviteContext(req, User.GetAdminId(), User.GetAdminEmail());

        var result = await CreateAdminInvite(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
