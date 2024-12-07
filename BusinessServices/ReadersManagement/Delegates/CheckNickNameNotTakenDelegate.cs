using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IResult<NickNameNotTakenCheckResultDto>> CheckNickNameNotTakenDelegate(CheckNickNameNotTakenContext context, CancellationToken token = default);