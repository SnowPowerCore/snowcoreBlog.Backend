using System.Text.Json.Serialization;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints;

/// <summary>
/// Serializes the <see cref="ApiResponse"/> envelope through the source-generated
/// <see cref="CoreSerializationContext"/> exactly like the previous FastEndpoints
/// <c>ResponseSerializer</c> (Web defaults: camelCase, indented, string enums,
/// ignore-null) so the wire format stays byte-compatible for frontend consumers.
/// A null payload produces an empty body, matching FastEndpoints behavior.
/// </summary>
public sealed class ApiResponseResult(ApiResponse? payload, int statusCode) : IResult
{
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = statusCode;

        if (payload is null)
        {
            return Task.CompletedTask;
        }

        return httpContext.Response.WriteAsJsonAsync(
            value: payload,
            type: typeof(ApiResponse),
            context: CoreSerializationContext.Default,
            cancellationToken: httpContext.RequestAborted);
    }
}
