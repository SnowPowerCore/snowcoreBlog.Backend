using FastEndpoints;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Endpoints;

public class GetIpRestrictionsEndpoint(IIpRestrictionRepository repo) : EndpointWithoutRequest<IReadOnlyList<IpRestrictionEntity>>
{
    public override void Configure()
    {
        Get("/ip-restrictions");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var all = await repo.GetAllAsync();
        await Send.ResponseAsync(all, cancellation: ct);
    }
}