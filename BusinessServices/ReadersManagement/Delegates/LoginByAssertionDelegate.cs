using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Delegates;

public delegate Task<IResult<LoginByAssertionResultDto>> LoginByAssertionDelegate(LoginByAssertionContext context, CancellationToken token = default);