using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.Steps;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles;

public class CreateArticleEndpoint : Endpoint<CreateArticleDto, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateAuthorAccountStep),
        typeof(GenerateSlugStep),
        typeof(SaveArticleStep)
    ])]
    protected CreateArticleDelegate CreateArticle { get; } = default!;

    public override void Configure()
    {
        Post("create");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        Claims("authorAccount");
        Description(b => b
            .WithTags(ApiTagConstants.Articles)
            .Accepts<CreateArticleDto>(MediaTypeNames.Application.Json)
            .Produces<ApiResponseForOpenApi<CreateArticleResultDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateArticleDto req, CancellationToken ct)
    {
        var userId = User?.FindFirst("sub")?.Value;
        var authorUserId = Guid.TryParse(userId, out var u) ? u : Guid.Empty;

        var context = new CreateArticleContext(req, authorUserId);
        var result = await CreateArticle(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}