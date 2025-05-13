using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.ReadersManagement.Options;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class UserTokenExtensions
{
    public static partial GetUserTokenPairWithPayload ToGetUserTokenPairWithPayload(this ReaderAccountTokenRequirementOptions readerAccountTokenRequirements);
}