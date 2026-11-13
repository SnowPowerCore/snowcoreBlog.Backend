using MaybeResults;
using snowcoreBlog.Backend.ApiAccessRestrictions.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Delegates;

public delegate Task<IMaybe<ApiAccessRestrictionsSnapshotDto>> GetApiAccessRestrictionsSnapshotDelegate(GetApiAccessRestrictionsSnapshotContext context, CancellationToken token = default);
