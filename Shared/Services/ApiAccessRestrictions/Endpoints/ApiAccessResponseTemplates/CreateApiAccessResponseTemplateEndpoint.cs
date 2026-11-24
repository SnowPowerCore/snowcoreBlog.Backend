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
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

/// <summary>
/// Admin: creates a response template and notifies the gateway about the change.
/// Simple CRUD write (map, persist, publish) — no branching, so it consumes the
/// repository directly (non-stepified).
/// </summary>
public class CreateApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository templateRepository,
                                                     IPublishEndpoint publisher)
    : Endpoint<CreateApiAccessResponseTemplateDto, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

    public override void Configure()
    {
        Post("templates");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Description(b => b
            .WithTags("ApiAccessResponseTemplates")
            .Produces<ApiResponseForOpenApi<ApiAccessResponseTemplateDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CreateApiAccessResponseTemplateDto req, CancellationToken ct)
    {
        var entity = req.ToEntity();
        var stored = await templateRepository.AddOrUpdateAsync(entity, token: ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);

        var result = Maybe.Create(stored.ToDto());

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
