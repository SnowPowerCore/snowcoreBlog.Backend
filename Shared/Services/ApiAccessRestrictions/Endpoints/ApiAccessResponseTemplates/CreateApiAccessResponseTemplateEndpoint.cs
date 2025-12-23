using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Entities;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class CreateApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo) : Endpoint<CreateApiAccessResponseTemplateDto, ApiAccessResponseTemplateEntity>
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
        await Send.CreatedAtAsync<GetApiAccessResponseTemplatesEndpoint>(entity, cancellation: ct);
    }
}