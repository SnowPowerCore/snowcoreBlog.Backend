using snowcoreBlog.Backend.IAM.Core.Contracts;

namespace snowcoreBlog.Backend.IAM.Core.Interfaces.Services.Password;

public interface IPasswordHasher
{
    string HashPassword(string password);

    PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword);
}