using snowcoreBlog.Backend.ReadersManagement.Core.Entities.Reader;
using snowcoreBlog.Backend.Shared.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;

public interface IReaderRepository : IRepository<ReaderEntity>;