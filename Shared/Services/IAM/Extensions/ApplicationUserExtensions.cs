using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

[Mapper]
public static partial class ApplicationUserExtensions
{
    private static partial ApplicationUserEntity MapperToUserEntity(this ApplicationTempUserEntity tempEntity);

    public static ApplicationUserEntity ToUserEntity(
        this ApplicationTempUserEntity tempEntity,
        Fido2PublicKeyCredentialEntity publicKeyCredentialEntity,
        bool emailConfirmed = true)
    {
        var userEntity = MapperToUserEntity(tempEntity);
        userEntity.PublicKeyCredentials = [publicKeyCredentialEntity];
        userEntity.EmailConfirmed = emailConfirmed;
        return userEntity;
    }
}