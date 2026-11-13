using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Admin;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AdminsManagement.Extensions;

[Mapper]
public static partial class AdminAccountMapper
{
    public static partial AdminDto ToDto(this AdminEntity entity);

    public static partial AdminInviteDto ToDto(this AdminInviteEntity entity);
}
