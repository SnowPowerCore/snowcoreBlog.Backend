using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class UserTokenExtensions
{
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