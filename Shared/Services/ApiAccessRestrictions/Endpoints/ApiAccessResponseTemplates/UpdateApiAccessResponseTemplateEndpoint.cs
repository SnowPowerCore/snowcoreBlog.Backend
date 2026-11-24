using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessResponseTemplates.Update;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

/// <summary>
/// Admin: updates a response template. Stepified: validates the route/body id pair,
/// resolves the stored template (404-style typed error on miss) and applies the update.
/// </summary>
public class UpdateApiAccessResponseTemplateEndpoint : Endpoint<UpdateApiAccessResponseTemplateDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateApiAccessResponseTemplateIdMatchesStep),
        typeof(ResolveApiAccessResponseTemplateForUpdateStep),
        typeof(UpdateApiAccessResponseTemplateStep),
    ])]
    public UpdateApiAccessResponseTemplateDelegate UpdateApiAccessResponseTemplate { get; } = default!;

    public override void Configure()
    {
        Put("templates/{id:guid}");
        Version(1);
        SerializerContext(ApiAccessRestrictionsSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags("ApiAccessResponseTemplates")
            .Produces<ApiResponseForOpenApi<ApiAccessResponseTemplateDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.BadRequest, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(UpdateApiAccessResponseTemplateDto req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await UpdateApiAccessResponseTemplate(new UpdateApiAccessResponseTemplateContext(req, id), ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
