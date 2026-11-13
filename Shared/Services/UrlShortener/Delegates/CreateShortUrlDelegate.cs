using MaybeResults;
using snowcoreBlog.Backend.UrlShortener.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.UrlShortener.Delegates;

public delegate Task<IMaybe<CreateShortUrlResult>> CreateShortUrlDelegate(CreateShortUrlContext context, CancellationToken token = default);
