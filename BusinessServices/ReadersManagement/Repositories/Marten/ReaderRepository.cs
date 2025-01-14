﻿using Marten;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.ReadersManagement.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.ReadersManagement.Repositories.Marten;

public class ReaderRepository(IDocumentSession session) : BaseMartenRepository<ReaderEntity>(session), IReaderRepository
{
}