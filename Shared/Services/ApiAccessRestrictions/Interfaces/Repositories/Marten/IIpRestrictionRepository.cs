using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;

public interface IIpRestrictionRepository : IRepository<IpRestrictionEntity>;