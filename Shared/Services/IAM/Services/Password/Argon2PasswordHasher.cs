using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.Backend.IAM.Core.Interfaces.Services.Password;
using snowcoreBlog.Backend.IAM.Models;
using snowcoreBlog.Backend.IAM.Options;
using Sodium;

namespace snowcoreBlog.Backend.IAM.Services.Password;

/// <summary>
/// Implements the standard Identity password hashing.
/// </summary>
public class Argon2PasswordHasher : IPasswordHasher
{
    private readonly PasswordHash.StrengthArgon _strength;

    public Argon2PasswordHasher(IOptions<Argon2PasswordHasherOptions> optionsAccessor = null)
    {
        var options = optionsAccessor?.Value ?? new Argon2PasswordHasherOptions();
        _strength = (object)options.Strength switch
        {
            Argon2HashStrength.Interactive => PasswordHash.StrengthArgon.Interactive,
            Argon2HashStrength.Moderate => PasswordHash.StrengthArgon.Moderate,
            Argon2HashStrength.Sensitive => PasswordHash.StrengthArgon.Sensitive,
            _ => throw new ArgumentOutOfRangeException(
                $"Invalid argon strength source: {options.Strength}."),
        };
    }

    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        var hash = PasswordHash.ArgonHashString(password, _strength).Replace("\0", string.Empty);
        return hash;
    }

    public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword)) throw new ArgumentNullException(nameof(hashedPassword));
        if (string.IsNullOrEmpty(providedPassword)) throw new ArgumentNullException(nameof(providedPassword));

        var isValid = PasswordHash.ArgonHashStringVerify(hashedPassword, providedPassword);

        return isValid ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
    }
}