namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints;

/// <summary>
/// Wire-compatibility helper preserving the exact response shape previously produced
/// by FastEndpoints <c>Send.ErrorsAsync</c> for the snapshot fallback path.
/// The CRUD endpoints migrated to the <see cref="PublicApi.Utilities.Api.ApiResponse"/>
/// envelope (see <see cref="EnvelopeResultExtensions"/>) on 2026-11-11; the former raw-shape
/// helpers (<c>Ok</c>, <c>NotFound</c>) were removed with that change.
/// </summary>
public static class EndpointResults
{
    /// <summary>
    /// The response shape FastEndpoints produced for <c>Send.ErrorsAsync(statusCode)</c>
    /// when no validation failures were collected: a bare error envelope with an empty errors map.
    /// </summary>
    public static IResult Errors(int statusCode) =>
        Results.Json(new ErrorResponseWire(
            StatusCode: statusCode,
            Message: "One or more errors occurred!",
            Errors: new Dictionary<string, List<string>>()), statusCode: statusCode);
}

/// <summary>
/// Wire shape of the FastEndpoints <c>ErrorResponse</c> contract (serialized camelCase by the
/// JSON options configured in <see cref="Extensions.Startup.HostingConfigurationExtensions"/>).
/// Property order matters: StatusCode, Message, Errors.
/// </summary>
public sealed record ErrorResponseWire(int StatusCode, string Message, Dictionary<string, List<string>> Errors);
