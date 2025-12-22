using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.Backend.AuthorsManagement.Delegates;
using snowcoreBlog.Backend.AuthorsManagement.Steps;
using snowcoreBlog.Backend.Core.Constants;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AuthorsManagement.Endpoints;

public class BecomeAuthorAccountEndpoint : Endpoint<BecomeAuthorAccountRequestDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(CreateAuthorEntityForExistingUserStep)
    ])]
    protected BecomeAuthorAccountDelegate BecomeAuthorAccount { get; } = default!;

    public override void Configure()
    {
        Post("author/become");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        Claims(ReaderAccountClaimConstants.ReaderAccountReaderAccountClaimKey);
        Description(b => b
            .WithTags("AuthorManagement")
            .Accepts<BecomeAuthorAccountRequestDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AuthorDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.Forbidden, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(BecomeAuthorAccountRequestDto req, CancellationToken ct)
    {
        if (User?.HasClaim(c => c.Type == "authorAccount") ?? false)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(["Already an author"], (int)HttpStatusCode.Forbidden),
                (int)HttpStatusCode.Forbidden,
                ct);
            return;
        }

        var context = new BecomeAuthorAccountContext(req.UserId, req.DisplayName);
        var result = await BecomeAuthorAccount(context, ct);
        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}