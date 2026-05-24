using MaybeResults;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Models;

namespace snowcoreBlog.Backend.UrlShortener.Delegates;

public delegate Task<IMaybe<ResolveShortUrlOperationResult>> ResolveShortUrlDelegate(ResolveShortUrlContext context, CancellationToken token = default);
