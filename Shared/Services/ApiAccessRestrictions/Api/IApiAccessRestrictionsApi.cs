using Apizr.Configuring.Request;
using Refit;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Api;

/// <summary>
/// Typed Refit contract for the ApiAccessRestrictions service, consumed in-process
/// by the gateway's snapshot refresher through an Apizr manager (same experience as
/// the frontend ApizrManager usage). Serves GetApiAccessRestrictionsSnapshotEndpoint
/// (GET snapshot, version 1 → /snapshot/v1).
/// </summary>
public interface IApiAccessRestrictionsApi
{
    [Headers("Accept: application/json, application/problem+json")]
    [Get("/snapshot/v1")]
    Task<ApiAccessRestrictionsSnapshotDto> GetSnapshot([RequestOptions] IApizrRequestOptions options);
}