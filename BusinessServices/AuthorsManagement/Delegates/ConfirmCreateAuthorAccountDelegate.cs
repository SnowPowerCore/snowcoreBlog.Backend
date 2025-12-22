using MaybeResults;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AuthorsManagement.Delegates;

public delegate Task<IMaybe<AuthorDto>> BecomeAuthorAccountDelegate(BecomeAuthorAccountContext context, CancellationToken token = default);
