using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;
using snowcoreBlog.Backend.ApiAccessRestrictions.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Steps.ApiAccess.Snapshot;

public sealed class LoadApiAccessSnapshotDataStep(
    IIpRestrictionRepository ipRepo,
    IRegionRestrictionRepository regionRepo,
    IApiAccessRuleRepository ruleRepo,
    IApiAccessResponseTemplateRepository templateRepo)
    : IStep<GetApiAccessRestrictionsSnapshotDelegate, GetApiAccessRestrictionsSnapshotContext, IMaybe<ApiAccessRestrictionsSnapshotDto>>
{
    public async Task<IMaybe<ApiAccessRestrictionsSnapshotDto>> InvokeAsync(
        GetApiAccessRestrictionsSnapshotContext context,
        GetApiAccessRestrictionsSnapshotDelegate next,
        CancellationToken token = default)
    {
        context.Rules = await ruleRepo.GetAllAsync(token);
        context.Templates = await templateRepo.GetAllAsync(token);
        context.IpRestrictions = await ipRepo.GetAllAsync(token);
        context.RegionRestrictions = await regionRepo.GetAllAsync(token);

        return await next(context, token);
    }
}
