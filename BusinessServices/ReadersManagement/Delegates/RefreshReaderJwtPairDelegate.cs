using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Models;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IMaybe<RefreshReaderJwtPairOperationResult>> RefreshReaderJwtPairDelegate(RefreshReaderJwtPairContext context, CancellationToken token = default);
