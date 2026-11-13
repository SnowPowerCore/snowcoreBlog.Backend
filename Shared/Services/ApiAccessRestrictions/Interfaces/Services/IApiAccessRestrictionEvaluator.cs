using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Services;

public interface IApiAccessRestrictionEvaluator
{
    Task<CheckApiAccessResponseDto> EvaluateAsync(CheckApiAccessRequestDto request, CancellationToken ct = default);
}
