using MaybeResults;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.Backend.UrlShortener.Models;

namespace snowcoreBlog.Backend.UrlShortener.Delegates;

public delegate Task<IMaybe<CreateShortUrlOperationResult>> CreateShortUrlDelegate(CreateShortUrlContext context, CancellationToken token = default);
