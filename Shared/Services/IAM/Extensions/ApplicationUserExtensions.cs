using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

public static class ApplicationUserExtensions
{
    public static ApplicationUser ToEntity(this CreateUser createUser) =>
        new()
        {
            UserName = createUser.NickName,
            FirstName = createUser.FirstName,
            LastName = !string.IsNullOrEmpty(createUser.LastName) ? createUser.LastName : string.Empty,
            Email = createUser.Email,
            PhoneNumber = createUser.PhoneNumber,
            AccessFailedCount = 0,
            PhoneNumberConfirmed = false,
            EmailConfirmed = false
        };
}