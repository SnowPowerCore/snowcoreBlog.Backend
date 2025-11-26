using Apizr.Configuring.Request;
using Refit;

namespace snowcoreBlog.Backend.Email.Api
{
    public interface IEmailDisposableApi
    {
        [Get("/disposable-email-domains/cache/{prefix}.json")]
        Task<HttpResponseMessage> GetDisposableDomainsAsync(string prefix, [RequestOptions] IApizrRequestOptions options);
    }
}