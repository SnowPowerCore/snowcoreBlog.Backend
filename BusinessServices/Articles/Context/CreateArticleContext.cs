using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Articles.Context;

public class CreateArticleContext
{
    public CreateArticleDto CreateRequest { get; }
    public Guid AuthorUserId { get; }
    public Guid[] ResolvedAuthorUserIds { get; }

    public CreateArticleContext(CreateArticleDto req, Guid authorUserId)
    {
        CreateRequest = req;
        AuthorUserId = authorUserId;
        ResolvedAuthorUserIds = req.Authors is { Length: > 0 } ? req.Authors : [authorUserId];
    }
}