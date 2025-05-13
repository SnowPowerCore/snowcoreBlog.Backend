using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

public static class UserTokenExtensions
{
    public static GetUserTokenPairWithPayload ToGetUserTokenPairWithPayload(this ReaderAccountTokenRequirementOptions readerAccountTokenRequirements) =>
        new()
        {
            Roles = readerAccountTokenRequirements.Roles.ToList(),
            Permissions = readerAccountTokenRequirements.Permissions.ToList(),
            Claims = new Dictionary<string, string>(readerAccountTokenRequirements.Claims),
            AccessTokenValidityDurationInMinutes = readerAccountTokenRequirements.AccessTokenValidityDurationInMinutes,
            RefreshTokenValidityDurationInMinutes = readerAccountTokenRequirements.RefreshTokenValidityDurationInMinutes
        };
}