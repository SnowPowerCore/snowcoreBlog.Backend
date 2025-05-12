using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

[Mapper]
public static partial class ApplicationUserExtensions
{
    private static partial ApplicationUserEntity MapperToUserEntity(this ApplicationTempUserEntity tempEntity);

    public static ApplicationUserEntity ToUserEntity(
        this ApplicationTempUserEntity tempEntity,
        Guid userId,
        Fido2PublicKeyCredentialEntity publicKeyCredentialEntity,
        bool emailConfirmed = true)
    {
        var userEntity = MapperToUserEntity(tempEntity);
        userEntity.Id = userId.ToString();
        if (userEntity.PublicKeyCredentials is not default(ICollection<Guid>))
            userEntity.PublicKeyCredentials.Add(publicKeyCredentialEntity.Id);
        else
            userEntity.PublicKeyCredentials = [publicKeyCredentialEntity.Id];
        userEntity.EmailConfirmed = emailConfirmed;
        return userEntity;
    }
}