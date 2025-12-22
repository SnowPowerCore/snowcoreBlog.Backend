using MinimalStepifiedSystem.Base;
using snowcoreBlog.Backend.Infrastructure;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public sealed class RefreshReaderJwtPairContext(string refreshTokenFromBody) : BaseGenericContext
{
    public string RefreshTokenFromBody { get; } = refreshTokenFromBody;

    public string? RefreshTokenFromCookie { get; set; }

    public string? RefreshToken { get; set; }

    public string? LockKey { get; set; }

    public ReaderRefreshTokenRecord? Record { get; set; }

    public AspireYarpGateway.Core.Contracts.UserTokenPairWithPayloadGenerated? NewPair { get; set; }

    public Models.RefreshReaderJwtPairOperationResult? Result { get; set; }
}
