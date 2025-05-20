using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class UserTokenExtensions
{
    [MapperIgnoreTarget(nameof(GetUserTokenPairWithPayload.Roles))]
    [MapperIgnoreTarget(nameof(GetUserTokenPairWithPayload.Permissions))]
    [MapperIgnoreTarget(nameof(GetUserTokenPairWithPayload.Claims))]
    private static partial GetUserTokenPairWithPayload MapperToGetUserTokenPairWithPayload(
        this ReaderAccountTokenRequirementOptions readerAccountTokenRequirements);

    public static GetUserTokenPairWithPayload ToGetUserTokenPairWithPayload(this ReaderAccountTokenRequirementOptions readerAccountTokenRequirements)
    {
        var payload = MapperToGetUserTokenPairWithPayload(readerAccountTokenRequirements);
        payload = payload with
        {
            Roles = readerAccountTokenRequirements.Roles.ToList(),
            Permissions = readerAccountTokenRequirements.Permissions.ToList(),
            Claims = readerAccountTokenRequirements.Claims.ToDictionary(),
        };
        return payload;
    }
}