using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.ReadersManagement.Options;
using snowcoreBlog.Backend.YarpGateway.Core.Contracts;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class UserTokenExtensions
{
    public static partial GetUserTokenPairWithPayload ToGetUserTokenPairWithPayload(this ReaderAccountTokenRequirementOptions readerAccountTokenRequirements);
}