using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Articles.Steps;

public class SaveArticleStep : IStep<CreateArticleDelegate, CreateArticleContext, IMaybe<CreateArticleResultDto>>
{
    public Task<IMaybe<CreateArticleResultDto>> InvokeAsync(CreateArticleContext context, CreateArticleDelegate next, CancellationToken token = default)
    {
        // The actual save happens in GenerateSlugStep in this scaffold. Delegate to next to preserve pipeline compatibility.
        return next(context, token);
    }
}
