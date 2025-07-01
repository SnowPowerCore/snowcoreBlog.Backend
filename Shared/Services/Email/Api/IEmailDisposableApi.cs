using Refit;
using Apizr.Configuring.Request;

namespace snowcoreBlog.Backend.Email.Api
{
    public interface IEmailDisposableApi
    {
        [Get("/disposable-email-domains/cache/{prefix}.json")]
        Task<HttpResponseMessage> GetDisposableDomainsAsync(string prefix, [RequestOptions] IApizrRequestOptions options);
    }
}