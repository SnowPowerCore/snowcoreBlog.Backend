using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Services.Password;
using snowcoreBlog.Backend.Infrastructure.Extensions;
using snowcoreBlog.Backend.Push.Models;
using snowcoreBlog.Backend.Push.Options;
using Sodium;

namespace snowcoreBlog.Backend.Push.Services.Password;

public class Argon2PasswordHasher : IPasswordHasher
{
    private readonly PasswordHash.StrengthArgon _strength;

    public Argon2PasswordHasher(IOptions<Argon2PasswordHasherOptions>? optionsAccessor = null)
    {
        var options = optionsAccessor?.Value ?? new Argon2PasswordHasherOptions();
        _strength = options.Strength switch
        {
            Argon2HashStrength.Interactive => PasswordHash.StrengthArgon.Interactive,
            Argon2HashStrength.Moderate => PasswordHash.StrengthArgon.Moderate,
            Argon2HashStrength.Sensitive => PasswordHash.StrengthArgon.Sensitive,
            _ => PasswordHash.StrengthArgon.Moderate,
        };
    }

    public string HashPassword(string password)
    {
        var passwordToBeHashed = !string.IsNullOrEmpty(password) ? password : StringExtensions.RandomString(20);
        return PasswordHash.ArgonHashString(passwordToBeHashed, _strength).Replace("\0", string.Empty);
    }

    public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword)) return PasswordVerificationResult.Failed;
        if (string.IsNullOrEmpty(providedPassword)) return PasswordVerificationResult.Failed;

        var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword);
        return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}