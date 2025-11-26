using Apizr.Configuring.Request;
using Refit;

namespace snowcoreBlog.Backend.Email.Api
{
    public interface IStaticEmailDisposableApi
    {
        [Get("/disposable/disposable-email-domains/master/domains.json")]
        Task<HttpResponseMessage> GetFallbackDomainsAsync([RequestOptions] IApizrRequestOptions options);
    }
}