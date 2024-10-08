using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IResult<ReaderAccountCreationResultDto>> CreateReaderAccountDelegate(CreateReaderAccountContext context, CancellationToken token = default);