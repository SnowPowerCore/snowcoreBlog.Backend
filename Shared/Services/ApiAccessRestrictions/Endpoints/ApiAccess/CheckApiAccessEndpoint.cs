using System.Net;
using System.Net.Mime;
using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints.ApiAccess;

/// <summary>
/// Public, unauthenticated surface used for API access restriction checks.
/// Returns the raw evaluation decision (no ApiResponse envelope) — callers
/// deserialize the decision object directly.
/// </summary>
public class CheckApiAccessEndpoint(IApiAccessRestrictionEvaluator evaluator)
    : Endpoint<CheckApiAccessRequestDto, CheckApiAccessResponseDto>
{
    public override void Configure()
    {
        Post("check");
        AllowAnonymous();
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        Description(b => b
            .WithTags("ApiAccess")
            .Produces<CheckApiAccessResponseDto>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json));
    }

    public override async Task HandleAsync(CheckApiAccessRequestDto req, CancellationToken ct)
    {
        var result = await evaluator.EvaluateAsync(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}
