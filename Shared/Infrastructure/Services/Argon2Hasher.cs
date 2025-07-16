using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Core.Interfaces.Services;
using snowcoreBlog.Backend.Core.Utilities;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using Sodium;

namespace snowcoreBlog.Backend.Infrastructure.Services;

/// <summary>
/// Implements the standard string hashing.
/// </summary>
public class Argon2Hasher : IHasher
{
    private readonly PasswordHash.StrengthArgon _strength;

    public Argon2Hasher(IOptions<Argon2StringHasherOptions> optionsAccessor = null)
    {
        var options = optionsAccessor?.Value ?? new Argon2StringHasherOptions();
        _strength = (object)options.Strength switch
        {
            Argon2HashStrength.Interactive => PasswordHash.StrengthArgon.Interactive,
            Argon2HashStrength.Moderate => PasswordHash.StrengthArgon.Moderate,
            Argon2HashStrength.Sensitive => PasswordHash.StrengthArgon.Sensitive,
            _ => throw new ArgumentOutOfRangeException(
                $"Invalid argon strength source: {options.Strength}."),
        };
    }

    public string Hash(string target)
    {
        if (string.IsNullOrWhiteSpace(target))
            throw new ArgumentNullException(nameof(target));

        return PasswordHash.ArgonHashString(target, _strength).Replace("\0", string.Empty);
    }

    public HashedStringsVerificationResult VerifyHashedStrings(string alreadyHashedString, string targetString)
    {
        if (string.IsNullOrWhiteSpace(alreadyHashedString)) throw new ArgumentNullException(nameof(alreadyHashedString));
        if (string.IsNullOrWhiteSpace(targetString)) throw new ArgumentNullException(nameof(targetString));

        var isValid = PasswordHash.ArgonHashStringVerify(alreadyHashedString, targetString);

        return isValid ? HashedStringsVerificationResult.Success : HashedStringsVerificationResult.Failed;
    }
}