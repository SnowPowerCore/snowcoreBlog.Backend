using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

[Mapper]
public static partial class ApplicationUserExtensions
{
    private static partial ApplicationUserEntity MapperToUserEntity(this ApplicationTempUserEntity tempEntity, string id, bool emailConfirmed);

    public static ApplicationUserEntity ToUserEntity(
        this ApplicationTempUserEntity tempEntity,
        Guid userId,
        Fido2PublicKeyCredentialEntity publicKeyCredentialEntity,
        bool emailConfirmed = true)
    {
        var userEntity = ToUserEntity(tempEntity, userId, emailConfirmed);
        if (userEntity.PublicKeyCredentials is not default(ICollection<Guid>))
            userEntity.PublicKeyCredentials.Add(publicKeyCredentialEntity.Id);
        else
            userEntity.PublicKeyCredentials = [publicKeyCredentialEntity.Id];
        return userEntity;
    }

    /// <summary>
    /// Creates a user entity from a pending temp user without a passkey
    /// credential — used by the authenticator (TOTP) registration path.
    /// </summary>
    public static ApplicationUserEntity ToUserEntity(
        this ApplicationTempUserEntity tempEntity,
        Guid userId,
        bool emailConfirmed = true)
    {
        var userEntity = MapperToUserEntity(tempEntity, userId.ToString(), emailConfirmed);
        userEntity.TwoFactorEnabled = !string.IsNullOrWhiteSpace(userEntity.TotpSecret);
        return userEntity;
    }
}