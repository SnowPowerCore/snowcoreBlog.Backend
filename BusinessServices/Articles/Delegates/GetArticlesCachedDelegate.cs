using MaybeResults;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Articles.Delegates;

public delegate Task<IMaybe<List<ArticleDto>>> GetArticlesCachedDelegate(GetArticlesCachedContext context, CancellationToken token = default);
