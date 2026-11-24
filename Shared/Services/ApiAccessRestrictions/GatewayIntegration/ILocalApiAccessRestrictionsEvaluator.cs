using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.GatewayIntegration;

public interface ILocalApiAccessRestrictionsEvaluator
{
    CheckApiAccessResponseDto Evaluate(CompiledApiAccessRestrictionsSnapshot snapshot, CheckApiAccessRequestDto request);
}
