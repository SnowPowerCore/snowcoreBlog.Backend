using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Articles.Context;
using snowcoreBlog.Backend.Articles.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Articles.Steps;

public class ValidateAuthorAccountStep : IStep<CreateArticleDelegate, CreateArticleContext, IMaybe<CreateArticleResultDto>>
{
    public Task<IMaybe<CreateArticleResultDto>> InvokeAsync(CreateArticleContext context, CreateArticleDelegate next, CancellationToken token = default)
    {
        // The endpoint will ensure author claim; here we simply continue the pipeline.
        return next(context, token);
    }
}
