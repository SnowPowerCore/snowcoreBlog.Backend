using Marten;
using snowcoreBlog.Backend.Core;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.ReadersManagement;

public class ReaderRepository(IDocumentSession session) : BaseMartenRepository<ReaderEntity>(session), IReaderRepository
{
}