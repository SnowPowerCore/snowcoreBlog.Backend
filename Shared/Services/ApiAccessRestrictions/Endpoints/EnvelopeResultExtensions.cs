using MaybeResults;
using snowcoreBlog.PublicApi.Extensions;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints;

/// <summary>
/// Maps a stepified-flow result to the standard <see cref="PublicApi.Utilities.Api.ApiResponse"/>
/// envelope with the endpoint-level status code (Some → 200, None → 400, null → 500),
/// serialized through the configured HTTP JSON options.
/// </summary>
public static class EnvelopeResultExtensions
{
    public static IResult ToEnvelopeResult<T>(this IMaybe<T> result) where T : notnull =>
        Results.Json(
            result?.ToApiResponse(),
            statusCode: result?.ToStatusCode() ?? StatusCodes.Status500InternalServerError);
}
