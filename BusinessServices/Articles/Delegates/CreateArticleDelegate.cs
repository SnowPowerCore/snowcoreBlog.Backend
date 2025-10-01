using MaybeResults;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.Backend.Articles.Context;

namespace snowcoreBlog.Backend.Articles.Delegates;

public delegate Task<IMaybe<CreateArticleResultDto>> CreateArticleDelegate(CreateArticleContext context, CancellationToken token = default);
