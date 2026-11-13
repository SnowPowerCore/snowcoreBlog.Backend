using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.Backend.ReadersManagement.Steps.Tokens;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.ReadersManagement.Endpoints.Tokens;

public class RefreshReaderAccountTokenEndpoint : Endpoint<string, ApiResponse?>
{
    public required IOptions<JsonOptions> JsonOptions { get; set; }

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
        // Accepts an empty JSON body (cookie-only proactive rotation) instead of
        // failing with a JSON serializer error before the step pipeline runs.
        RequestBinder(new RefreshTokenRequestBinder());
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

        var result = await RefreshReaderJwtPair(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
