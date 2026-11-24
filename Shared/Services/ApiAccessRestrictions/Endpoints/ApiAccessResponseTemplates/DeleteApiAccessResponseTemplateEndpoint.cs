using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Delete;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

/// <summary>
/// Admin: deletes a response template. Stepified: resolves the stored template
/// (404-style typed error on miss), then removes it.
/// </summary>
public class DeleteApiAccessResponseTemplateEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ResolveApiAccessResponseTemplateForDeleteStep),
        typeof(DeleteApiAccessResponseTemplateStep),
    ])]
    public DeleteApiAccessResponseTemplateDelegate DeleteApiAccessResponseTemplate { get; } = default!;

    public override void Configure()
    {
        Delete("templates/{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags("ApiAccessResponseTemplates")
            .Produces<ApiResponseForOpenApi<ApiAccessResponseTemplateDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.BadRequest, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await DeleteApiAccessResponseTemplate(new DeleteApiAccessResponseTemplateContext(id), ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
