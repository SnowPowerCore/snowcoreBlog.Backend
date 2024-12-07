using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IResult<RequestReaderAccountCreationResultDto>> RequestCreateReaderAccountDelegate(RequestCreateReaderAccountContext context, CancellationToken token = default);