using FastEndpoints;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Endpoints;

public class GetIpRestrictionsEndpoint : EndpointWithoutRequest<IReadOnlyList<IpRestrictionEntity>>
{
    private readonly IIpRestrictionRepository _repo;

    public GetIpRestrictionsEndpoint(IIpRestrictionRepository repo)
    {
        _repo = repo;
    }

    public override void Configure()
    {
        Get("/ip-restrictions");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var all = await _repo.GetAllAsync();
        await SendAsync(all, cancellation: ct);
    }
}