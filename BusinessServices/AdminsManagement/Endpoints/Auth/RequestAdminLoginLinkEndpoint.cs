using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Marten;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Auth;

/// <summary>
/// Anonymous: requests a one-time admin login link by email. Always answers
/// 200 (no account enumeration); the link is only sent when an ACTIVE admin
/// with that email exists.
/// </summary>
public class RequestAdminLoginLinkEndpoint : Endpoint<RequestAdminLoginLinkDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public required AdminEmailPublisher EmailPublisher { get; set; }

    public required IOptions<AdminsManagementOptions> ModuleOptions { get; set; }

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
        var email = req.Email.Trim().ToLowerInvariant();

        var admin = await DocumentSession.Query<AdminEntity>()
            .FirstOrDefaultAsync(x =>
                x.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && x.IsActive, ct);

        if (admin is not default(AdminEntity))
        {
            var rawToken = AdminEmailPublisher.NewToken();
            DocumentSession.Insert(new AdminLoginTokenEntity
            {
                Email = email,
                TokenHash = AdminEmailPublisher.Sha256Hex(rawToken),
                CreatedAtUtc = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(ModuleOptions.Value.LoginLinkValidityDurationInMinutes),
            });
            await DocumentSession.SaveChangesAsync(ct);

            var loginUrl = $"{ModuleOptions.Value.AdminFrontendDomain.TrimEnd('/')}/login?token={rawToken}";
            await EmailPublisher.SendAsync(
                email,
                "Your admin sign-in link",
                $@"Hello,

                Use the link below to sign in to the snowcore's Blog admin portal:

                {loginUrl}

                The link is single-use and valid for {ModuleOptions.Value.LoginLinkValidityDurationInMinutes} minutes.
                If you did not request it, you can safely ignore this email.", CancellationToken.None);
        }

        await Send.ResponseAsync(
            new ApiResponse(StatusCode: (int)HttpStatusCode.OK),
            (int)HttpStatusCode.OK,
            ct);
    }
}
