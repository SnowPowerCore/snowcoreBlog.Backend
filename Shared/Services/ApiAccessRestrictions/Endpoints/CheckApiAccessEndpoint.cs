using FastEndpoints;
using snowcoreBlog.Backend.ApiAccessRestrictions.Services;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Endpoints;

public class CheckApiAccessEndpoint(IApiAccessRestrictionEvaluator evaluator) : Endpoint<CheckApiAccessRequestDto, CheckApiAccessResponseDto>
{
    public override void Configure()
    {
        Post("check");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CheckApiAccessRequestDto req, CancellationToken ct)
    {
        var result = await evaluator.EvaluateAsync(req, ct);
        await Send.ResponseAsync(result, cancellation: ct);
    }
}