using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MassTransit;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessRules;

/// <summary>
/// Admin: creates an API access rule and notifies the gateway about the change.
/// Simple CRUD write (map, persist, publish) вЂ” no branching, so it consumes the
/// repository directly (non-stepified).
/// </summary>
public class CreateApiAccessRuleEndpoint(IApiAccessRuleRepository ruleRepository,
                                         IPublishEndpoint publisher)
    : Endpoint<CreateApiAccessRuleDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Post("rules");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags("ApiAccessRules")
            .Produces<ApiResponseForOpenApi<ApiAccessRuleDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateApiAccessRuleDto req, CancellationToken ct)
    {
        var entity = req.ToEntity();
        var stored = await ruleRepository.AddOrUpdateAsync(entity, token: ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);

        var result = Maybe.Create(stored.ToDto());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
