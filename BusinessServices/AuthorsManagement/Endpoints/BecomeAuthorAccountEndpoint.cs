using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.Backend.AuthorsManagement.Delegates;
using snowcoreBlog.Backend.BusinessServices.AuthorsManagement.Steps;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.AuthorsManagement.Endpoints;

public class BecomeAuthorAccountEndpoint : Endpoint<BecomeAuthorAccountRequestDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(CreateAuthorEntityForExistingUserStep)
    ])]
    protected BecomeAuthorAccountDelegate BecomeAuthorAccount { get; }

    public override void Configure()
    {
        Post("author/become");
        Version(1);
        AllowAnonymous();
        EnableAntiforgery();
        Description(b => b
            .WithTags("AuthorManagement")
            .Accepts<BecomeAuthorAccountRequestDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<AuthorEntity>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(BecomeAuthorAccountRequestDto req, CancellationToken ct)
    {
        var context = new BecomeAuthorAccountContext(req.UserId, req.DisplayName);
        var result = await BecomeAuthorAccount(context, ct);
        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}

public record BecomeAuthorAccountRequestDto(Guid UserId, string DisplayName);
