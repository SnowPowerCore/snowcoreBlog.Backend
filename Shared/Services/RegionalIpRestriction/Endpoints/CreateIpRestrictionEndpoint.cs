using FastEndpoints;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Endpoints;

public class CreateIpRestrictionRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<string>? IpRanges { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}

public class CreateIpRestrictionEndpoint : Endpoint<CreateIpRestrictionRequest, IpRestrictionEntity>
{
    private readonly IIpRestrictionRepository _repo;

    public CreateIpRestrictionEndpoint(IIpRestrictionRepository repo)
    {
        _repo = repo;
    }

    public override void Configure()
    {
        Post("/ip-restrictions");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CreateIpRestrictionRequest req, CancellationToken ct)
    {
        var entity = new IpRestrictionEntity
        {
            Name = req.Name,
            Description = req.Description,
            IpRanges = req.IpRanges ?? [],
            ExpiresAt = req.ExpiresAt
        };

        await _repo.SaveAsync(entity);

        await Send.CreatedAtAsync<GetIpRestrictionsEndpoint>(entity, cancellation: ct);
    }
}