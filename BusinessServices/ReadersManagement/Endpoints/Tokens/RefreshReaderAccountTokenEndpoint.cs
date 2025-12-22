using System.Net;
using System.Net.Mime;
using FastEndpoints;
using MaybeResults;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Infrastructure;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Models;
using snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Tokens;

public class RefreshReaderAccountTokenEndpoint : Endpoint<string, ApiResponse?>
{
    public IOptions<JsonOptions> JsonOptions { get; set; } = default!;

    [StepifiedProcess(Steps = [
        typeof(ResolveRefreshTokenStep),
        typeof(UseRefreshTokenLockStep),
        typeof(ValidateRefreshTokenRecordStep),
        typeof(RotateReaderTokenPairStep),
    ])]
    protected RefreshReaderJwtPairDelegate RefreshReaderJwtPair { get; } = default!;

    public override void Configure()
    {
        Post("tokens/refresh");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Tokens)
            .Accepts<string>(MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.Unauthorized, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(string req, CancellationToken ct)
    {
        var context = new RefreshReaderJwtPairContext(req);
        var maybeResult = await RefreshReaderJwtPair(context, ct);

        // Steps always return a Some with an operation result (including error cases).
        var op = maybeResult is Some<RefreshReaderJwtPairOperationResult> s
            ? s.Value
            : new RefreshReaderJwtPairOperationResult
            {
                HttpStatusCode = 500,
                Response = new ApiResponse(default, 0, 500, [ApiResponseConstants.UnknownError])
            };

        await Send.ResponseAsync(op.Response, op.HttpStatusCode, ct);
    }
}
