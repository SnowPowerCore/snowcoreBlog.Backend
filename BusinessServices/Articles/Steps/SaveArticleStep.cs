using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Articles.Constants;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using StackExchange.Redis;

namespace snowcoreBlog.Backend.Articles.Steps;

public class SaveArticleStep(IConnectionMultiplexer redis) : IStep<CreateArticleDelegate, CreateArticleContext, IMaybe<CreateArticleResultDto>>
{
    public async Task<IMaybe<CreateArticleResultDto>> InvokeAsync(CreateArticleContext context, CreateArticleDelegate next, CancellationToken token = default)
    {
        var result = await next(context, token);

        // Evict the home-page articles cache so newly created articles appear immediately.
        if (result is Some<CreateArticleResultDto>)
        {
            try
            {
                await redis.GetDatabase().KeyDeleteAsync(ArticlesCacheKeys.Latest);
            }
            catch
            {
                // Eviction is best-effort; a stale entry expires via TTL anyway.
            }
        }

        return result;
    }
}
