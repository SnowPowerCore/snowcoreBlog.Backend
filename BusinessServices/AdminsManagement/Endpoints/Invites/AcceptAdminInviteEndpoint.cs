using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Steps.Invites.Accept;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Invites;

/// <summary>
/// Anonymous (token-gated): accepts a pending invite — creates the admin
/// entity with the STRONG reference to the inviting admin, consumes the
/// invite and signs the new admin in (admin auth cookie).
/// </summary>
public class AcceptAdminInviteEndpoint : Endpoint<AcceptAdminInviteDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ResolveAdminInviteStep),
        typeof(CreateAdminIdentityStep),
        typeof(CreateAdminEntityStep),
        typeof(ConsumeAdminInviteStep),
        typeof(IssueAdminSessionStep),
    ])]
    protected AcceptAdminInviteDelegate AcceptAdminInvite { get; } = default!;

    public override void Configure()
    {
        Post("invites/accept");
        Version(1);
        SerializerContext(AdminsManagementSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Accepts<AcceptAdminInviteDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AdminDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(AcceptAdminInviteDto req, CancellationToken ct)
    {
        var context = new AcceptAdminInviteContext(req.Token, req.NickName);

        var result = await AcceptAdminInvite(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
