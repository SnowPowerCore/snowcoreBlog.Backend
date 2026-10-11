using FastEndpoints;
using MassTransit;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class CreateApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo,
                                                     IPublishEndpoint publisher) : Endpoint<CreateApiAccessResponseTemplateDto, ApiAccessResponseTemplateEntity>
{
    public override void Configure()
    {
        Post("templates");
        AllowAnonymous(); // TODO: restrict to admin
        Version(1);
    }

    public override async Task HandleAsync(CreateApiAccessResponseTemplateDto req, CancellationToken ct)
    {
        var entity = new ApiAccessResponseTemplateEntity
        {
            Name = req.Name,
            StatusCode = req.StatusCode,
            ContentType = req.ContentType,
            BodyJson = req.BodyJson,
            Reason = req.Reason
        };

        await repo.SaveAsync(entity, ct);
        await publisher.Publish(new ApiAccessRestrictionsChanged(DateTimeOffset.UtcNow), ct);
        await Send.CreatedAtAsync<GetApiAccessResponseTemplatesEndpoint>(entity, cancellation: ct);
    }
}