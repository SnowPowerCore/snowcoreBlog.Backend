using Refit;
using Apizr.Configuring.Request;

namespace snowcoreBlog.Backend.Email.Api
{
    public interface IStaticEmailDisposableApi
    {
        [Get("/disposable/disposable-email-domains/master/domains.json")]
        Task<HttpResponseMessage> GetFallbackDomainsAsync([RequestOptions] IApizrRequestOptions options);
    }
}