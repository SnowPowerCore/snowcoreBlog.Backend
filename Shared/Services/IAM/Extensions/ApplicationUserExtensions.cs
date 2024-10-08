using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Entities;

namespace snowcoreBlog.Backend.IAM.Extensions;

public static class ApplicationUserExtensions
{
    public static ApplicationUser ToEntity(this CreateUser createUser) =>
        new()
        {
            UserName = $"{createUser.FirstName} {createUser.LastName}",
            Email = createUser.Email,
            PhoneNumber = createUser.PhoneNumber,
            AccessFailedCount = 0,
            PhoneNumberConfirmed = false,
            EmailConfirmed = false
        };
}