using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccessRules.Delete;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

/// <summary>
/// Admin: deletes an API access rule. Stepified: resolves the stored rule
/// (404-style typed error on miss), then removes it.
/// </summary>
public class DeleteApiAccessRuleEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ResolveApiAccessRuleForDeleteStep),
        typeof(DeleteApiAccessRuleStep),
    ])]
    public DeleteApiAccessRuleDelegate DeleteApiAccessRule { get; } = default!;

    public override void Configure()
    {
        Delete("rules/{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags("ApiAccessRules")
            .Produces<ApiResponseForOpenApi<ApiAccessRuleDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.BadRequest, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var result = await DeleteApiAccessRule(new DeleteApiAccessRuleContext(id), ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
