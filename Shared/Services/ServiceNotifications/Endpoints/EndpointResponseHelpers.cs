using FluentValidation.Results;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ServiceNotifications.Endpoints;

/// <summary>
/// Helpers replicating FastEndpoints' error <c>ResponseBuilder</c> output so validation
/// failures keep their exact wire shape (<see cref="ApiResponse"/> envelope, 400).
/// </summary>
public static class EndpointResponseHelpers
{
    public static ApiResponse ApiResponseFromFailures(IReadOnlyList<ValidationFailure> failures)
    {
        var failuresDict = failures
            .GroupBy(static f => f.PropertyName)
            .ToDictionary(
                keySelector: static g => g.Key,
                elementSelector: g => g.Select(m => $"{g.Key}: {m.ErrorMessage}").ToArray());

        return ErrorResponseUtilities.ApiResponseWithErrors(
            failuresDict.Values.SelectMany(static x => x.Select(static s => s)).ToList(),
            StatusCodes.Status400BadRequest);
    }
}
