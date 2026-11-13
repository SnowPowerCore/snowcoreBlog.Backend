using MaybeResults;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;

public delegate Task<IMaybe<ApiAccessResponseTemplateDto>> UpdateApiAccessResponseTemplateDelegate(UpdateApiAccessResponseTemplateContext context, CancellationToken token = default);
