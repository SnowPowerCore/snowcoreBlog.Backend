using MaybeResults;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.Backend.Core.Entities.Author;

namespace snowcoreBlog.Backend.AuthorsManagement.Delegates;

public delegate Task<IMaybe<AuthorEntity>> BecomeAuthorAccountDelegate(BecomeAuthorAccountContext context, CancellationToken token = default);
