using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class UpdateApiAccessResponseTemplateEndpoint(IApiAccessResponseTemplateRepository repo) : Endpoint<UpdateApiAccessResponseTemplateDto>
{
    public override void Configure()
    {
        Put("templates/{id:guid}");
        AllowAnonymous(); // TODO: restrict to admin
        Version(1);
    }

    public override async Task HandleAsync(UpdateApiAccessResponseTemplateDto req, CancellationToken ct)
    {
        var id = Route<Guid>("id");
        if (req.Id != id)
        {
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var existing = await repo.GetByIdAsync(id, ct);
        if (existing is null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        existing.Name = req.Name;
        existing.StatusCode = req.StatusCode;
        existing.ContentType = req.ContentType;
        existing.BodyJson = req.BodyJson;
        existing.Reason = req.Reason;

        await repo.SaveAsync(existing, ct);
        await Send.OkAsync(cancellation: ct);
    }
}