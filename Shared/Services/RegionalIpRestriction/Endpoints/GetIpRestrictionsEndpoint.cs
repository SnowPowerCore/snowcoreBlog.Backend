using FastEndpoints;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;

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
        await Send.ResponseAsync(all, cancellation: ct);
    }
}