using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Infrastructure.Utilities;

public static class ErrorResponseUtilities
{
    public static ApiResponse ApiResponseWithErrors(IReadOnlyCollection<string> errors, int statusCode) =>
        new(default, errors.Count, statusCode, errors);
}