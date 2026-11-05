using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MassTransit;
using Marten;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.AdminsManagement.Services;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Utilities.DataResult;

namespace snowcoreBlog.Backend.AdminsManagement.Endpoints.Auth;

/// <summary>
/// Anonymous (token-gated): exchanges a single-use emailed login token for
/// an admin session (admin auth cookie).
/// </summary>
public class ConfirmAdminLoginEndpoint : Endpoint<ConfirmAdminLoginDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public required IDocumentSession DocumentSession { get; set; }

    public required IRequestClient<ValidateAdminExists> ValidateAdminClient { get; set; }

    public required AdminTokenPairIssuer TokenIssuer { get; set; }

    public override void Configure()
    {
        Post("login/confirm");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
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
        var tokenHash = AdminTokenUtilities.Sha256Hex(req.Token);

        var loginToken = await DocumentSession.Query<AdminLoginTokenEntity>()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, ct);

        if (loginToken is default(AdminLoginTokenEntity) || !loginToken.IsUsable)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["This sign-in link is invalid, already used, or expired."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var admin = await DocumentSession.Query<AdminEntity>()
            .FirstOrDefaultAsync(x =>
                x.Email.Equals(loginToken.Email, StringComparison.OrdinalIgnoreCase) && x.IsActive, ct);

        if (admin is default(AdminEntity))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["This admin account no longer exists or is inactive."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        loginToken = loginToken with { UsedAtUtc = DateTime.UtcNow };
        DocumentSession.Update(loginToken);
        await DocumentSession.SaveChangesAsync(ct);

        // Double-check the identity still exists in IAM before issuing a session.
        var iamCheck = await ValidateAdminClient.GetResponse<DataResult<AdminExistsValidationResult>>(
            new ValidateAdminExists { Email = loginToken.Email }, ct);

        if (!iamCheck.Message.IsSuccess || iamCheck.Message.Value?.Exists is false)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["This admin identity no longer exists in IAM."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        TokenIssuer.IssueAndSetCookie(admin);

        var result = Maybe.Create(admin.ToDto());

        await Send.ResponseAsync(
            result.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
