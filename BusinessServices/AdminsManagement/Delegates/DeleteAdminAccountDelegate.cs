using MaybeResults;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Delegates;

public delegate Task<IMaybe<DeleteAdminAccountResultDto>> DeleteAdminAccountDelegate(DeleteAdminAccountContext context, CancellationToken token = default);
