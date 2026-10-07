using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.Steps.Articles;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles;

public class GetArticleBySlugEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(GetArticleBySlugStep)
    ])]
    protected GetArticleBySlugDelegate GetArticleBySlug { get; } = default!;

    public override void Configure()
    {
        Get("{slug}");
        ResponseCache(1800);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Articles)
            .Produces<ApiResponseForOpenApi<ArticleDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var slug = Route<string>("slug")!;
        var context = new GetArticleBySlugContext(slug);
        var result = await GetArticleBySlug(context, ct);

        if (result is not Some<ArticleDto> success)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Article with slug '{slug}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        await Send.ResponseAsync(
            result!.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result.ToStatusCode(),
            ct);
    }
}
