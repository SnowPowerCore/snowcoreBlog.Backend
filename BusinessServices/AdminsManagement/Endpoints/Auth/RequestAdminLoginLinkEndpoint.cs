using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.Backend.AdminsManagement.Delegates;
using snowcoreBlog.Backend.AdminsManagement.Steps.Auth.LoginLink;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Auth;

/// <summary>
/// Anonymous: requests a one-time admin login link by email.
/// The link is only sent when an ACTIVE admin with that email exists.
/// </summary>
public class RequestAdminLoginLinkEndpoint : Endpoint<RequestAdminLoginLinkDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ResolveActiveAdminByEmailStep),
        typeof(CreateAdminLoginTokenStep),
        typeof(SendAdminLoginLinkEmailStep),
    ])]
    protected RequestAdminLoginLinkDelegate RequestAdminLoginLink { get; } = default!;

    public override void Configure()
    {
        Post("login/link");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Administration)
            .Accepts<RequestAdminLoginLinkDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(RequestAdminLoginLinkDto req, CancellationToken ct)
    {
        var context = new RequestAdminLoginLinkContext(req.Email.Trim().ToLowerInvariant());

        var result = await RequestAdminLoginLink(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
