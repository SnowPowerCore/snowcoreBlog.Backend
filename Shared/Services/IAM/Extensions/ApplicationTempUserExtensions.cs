using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

public static class ApplicationTempUserExtensions
{
    public static ApplicationTempUserEntity ToEntity(
        this CreateTempUser createUser, string activationToken, DateTimeOffset activationTokenExpirationDate) =>
        new()
        {
            UserName = createUser.UserName,
            FirstName = createUser.FirstName,
            LastName = !string.IsNullOrEmpty(createUser.LastName) ? createUser.LastName : string.Empty,
            Email = createUser.Email,
            PhoneNumber = createUser.PhoneNumber,
            ActivationToken = activationToken,
            ActivationTokenExpirationDate = activationTokenExpirationDate,
            ConfirmedAgreement = createUser.ConfirmedAgreement,
            InitialEmailConsent = createUser.InitialEmailConsent,
        };
}