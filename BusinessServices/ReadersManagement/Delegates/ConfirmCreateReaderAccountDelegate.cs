using MaybeResults;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IMaybe<ReaderAccountCreatedDto>> ConfirmCreateReaderAccountDelegate(ConfirmCreateReaderAccountContext context, CancellationToken token = default);