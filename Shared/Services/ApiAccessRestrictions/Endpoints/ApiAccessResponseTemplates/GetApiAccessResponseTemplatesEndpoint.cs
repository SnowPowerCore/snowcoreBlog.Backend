using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

/// <summary>
/// Admin: lists all response templates. Short and stable by design, so it consumes
/// the repository directly (non-stepified).
/// </summary>
public class GetApiAccessResponseTemplatesEndpoint(IApiAccessResponseTemplateRepository templateRepository)
    : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Get("templates");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags("ApiAccessResponseTemplates")
            .Produces<ApiResponseForOpenApi<List<ApiAccessResponseTemplateDto>>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var templates = await templateRepository.GetAllAsync(ct);

        var result = Maybe.Create<IReadOnlyList<ApiAccessResponseTemplateDto>>(
            templates.Select(static t => t.ToDto()).ToList());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
