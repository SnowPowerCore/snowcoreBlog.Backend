using FastEndpoints;
using snowcoreBlog.PublicApi.Constants;

namespace snowcoreBlog.Backend.Infrastructure.Processors;

/// <summary>
/// Tag-driven attachment of the persist processors — mirrors the captcha precedent
/// (<c>RequireCaptchaVerification</c> → <c>AltchaVerificationProcessor</c>). The ONLY thing an
/// endpoint does to opt into durable request persistence is declare
/// <c>Tags(ApiTagConstants.Persist)</c>.
/// </summary>
public static class PersistRequestFastEndpointsExtensions
{
    public static void AttachPersistRequestProcessors(this EndpointDefinition ep)
    {
        if (ep.EndpointTags?.Contains(ApiTagConstants.Persist) ?? false)
        {
            ep.PreProcessor<PersistRequestPreProcessor>(Order.Before);
            ep.PostProcessor<PersistRequestPostProcessor>(Order.After);
        }
    }
}
