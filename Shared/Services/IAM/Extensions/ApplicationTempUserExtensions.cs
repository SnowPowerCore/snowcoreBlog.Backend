using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

[Mapper]
public static partial class ApplicationTempUserExtensions
{
    [MapProperty(nameof(CreateTempUser.LastName), nameof(ApplicationTempUserEntity.LastName), Use = nameof(MapLastName))]
    public static partial ApplicationTempUserEntity ToEntity(
        this CreateTempUser createUser, string activationToken, DateTimeOffset activationTokenExpirationDate);
    
    [UserMapping]
    private static string MapLastName(string? lastName) =>
        !string.IsNullOrEmpty(lastName) ? lastName : string.Empty;
}