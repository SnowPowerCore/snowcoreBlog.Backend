using Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Repositories.Marten;

public class IpRestrictionRepository(IDocumentSession session) : BaseMartenRepository<IpRestrictionEntity>(session), IIpRestrictionRepository;