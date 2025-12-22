using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Models;

public sealed record RefreshReaderJwtPairOperationResult
{
    public required ApiResponse Response { get; init; }

    public required int HttpStatusCode { get; init; }
}
