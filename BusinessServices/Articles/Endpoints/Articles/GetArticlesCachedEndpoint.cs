using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Steps.Articles;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.Json;

namespace snowcoreBlog.Backend.Articles.Endpoints.Articles;

public class GetArticlesCachedEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; }
    
    [StepifiedProcess(Steps = [
        typeof(GetArticlesCachedStep)
    ])]
    protected GetArticlesCachedDelegate GetArticlesCached { get; } = default!;

    public override void Configure()
    {
        Get("cached");
        ResponseCache(1800);
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        EnableAntiforgery();
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Articles)
            .Produces<ApiResponseForOpenApi<ArticleDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var context = new GetArticlesCachedContext();
        var result = await GetArticlesCached(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.OK,
            ct);
    }
}