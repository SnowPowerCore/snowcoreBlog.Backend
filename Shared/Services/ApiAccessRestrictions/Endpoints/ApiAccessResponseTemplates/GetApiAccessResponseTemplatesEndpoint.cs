using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Constants;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.ApiAccessRestrictions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccessResponseTemplates;

public class GetApiAccessResponseTemplatesEndpoint(IApiAccessResponseTemplateRepository repo) : EndpointWithoutRequest<IReadOnlyList<ApiAccessResponseTemplateEntity>>
{
    public override void Configure()
    {
        Get("templates");
        Claims(AdminClaimConstants.AdminAccountClaimKey);
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var all = await repo.GetAllAsync(ct);
        await Send.ResponseAsync(all.ToList(), cancellation: ct);
    }
}