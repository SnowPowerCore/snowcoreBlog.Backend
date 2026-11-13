using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IMaybe<RefreshReaderAccountTokenResultDto>> RefreshReaderJwtPairDelegate(RefreshReaderJwtPairContext context, CancellationToken token = default);
