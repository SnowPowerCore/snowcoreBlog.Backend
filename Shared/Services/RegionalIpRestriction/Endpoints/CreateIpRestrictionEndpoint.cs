using FastEndpoints;
using snowcoreBlog.Backend.RegionalIpRestriction.Entities;
using snowcoreBlog.Backend.RegionalIpRestriction.Repositories.Marten;

namespace snowcoreBlog.Backend.RegionalIpRestriction.Endpoints;

public class CreateIpRestrictionRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<string>? IpRanges { get; set; }
    public DateTimeOffset? ExpiresAt { get; set; }
}

public class CreateIpRestrictionEndpoint(IIpRestrictionRepository repo) : Endpoint<CreateIpRestrictionRequest, IpRestrictionEntity>
{
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

        await repo.SaveAsync(entity);

        await Send.CreatedAtAsync<GetIpRestrictionsEndpoint>(entity, cancellation: ct);
    }
}