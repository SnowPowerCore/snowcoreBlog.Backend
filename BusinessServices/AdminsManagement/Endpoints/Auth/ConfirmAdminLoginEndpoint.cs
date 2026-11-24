using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Steps.Auth.ConfirmLogin;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Auth;

/// <summary>
/// Anonymous (token-gated): exchanges a single-use emailed login token for
/// an admin session (admin auth cookie).
/// </summary>
public class ConfirmAdminLoginEndpoint : Endpoint<ConfirmAdminLoginDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ResolveAdminLoginTokenStep),
        typeof(ResolveAdminForLoginStep),
        typeof(ConsumeAdminLoginTokenStep),
        typeof(ValidateAdminExistsInIamStep),
        typeof(IssueAdminSessionStep),
    ])]
    protected ConfirmAdminLoginDelegate ConfirmAdminLogin { get; } = default!;

    public override void Configure()
    {
        Post("login/confirm");
        Version(1);
        SerializerContext(AdminsManagementSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Accepts<ConfirmAdminLoginDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AdminDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(ConfirmAdminLoginDto req, CancellationToken ct)
    {
        var context = new ConfirmAdminLoginContext(req.Token);

        var result = await ConfirmAdminLogin(context, ct);
        
        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
