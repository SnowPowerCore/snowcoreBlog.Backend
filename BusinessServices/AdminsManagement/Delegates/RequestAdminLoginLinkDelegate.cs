using MaybeResults;
using snowcoreBlog.Backend.AdminsManagement.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Delegates;

public delegate Task<IMaybe<RequestAdminLoginLinkResultDto>> RequestAdminLoginLinkDelegate(RequestAdminLoginLinkContext context, CancellationToken token = default);
