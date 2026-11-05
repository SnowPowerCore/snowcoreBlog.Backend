using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces;

public interface IApiAccessRestrictionEvaluator
{
    Task<CheckApiAccessResponseDto> EvaluateAsync(CheckApiAccessRequestDto request, CancellationToken ct = default);
}
