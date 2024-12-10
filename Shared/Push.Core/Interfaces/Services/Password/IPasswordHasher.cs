using Microsoft.AspNetCore.Identity;

namespace snowcoreBlog.Backend.Push.Core.Interfaces.Services.Password;

public interface IPasswordHasher
{
    string HashPassword(string password);

    PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
}