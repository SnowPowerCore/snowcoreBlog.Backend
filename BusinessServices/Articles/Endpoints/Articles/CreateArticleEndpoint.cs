using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.Steps;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles;

public class CreateArticleEndpoint : Endpoint<CreateArticleDto, ApiResponse?>
{
    [StepifiedProcess(Steps = [
        typeof(ValidateAuthorAccountStep),
        typeof(GenerateSlugStep),
        typeof(SaveArticleStep)
    ])]
    protected CreateArticleDelegate CreateArticle { get; } = default!;

    public override void Configure()
    {
        Post("articles/create");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        Claims("authorAccount");
        Description(b => b
            .WithTags(ApiTagConstants.ReaderAccountManagement)
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

        await SendAsync(result?.ToApiResponse(), result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError, ct);
    }
}