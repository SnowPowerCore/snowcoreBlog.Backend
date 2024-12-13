using snowcoreBlog.Backend.Core.Utilities;

namespace snowcoreBlog.Backend.Core.Interfaces.Services;

/// <summary>
/// Provides an abstraction for hashing strings. Do not use for passwords.
/// </summary>
public interface IHasher
{
    /// <summary>
    /// Returns a hashed representation of the supplied <paramref name="string"/> for the specified <paramref name="user"/>.
    /// </summary>
    /// <param name="string">The string to hash.</param>
    /// <returns>A hashed representation of the supplied <paramref name="string"/> for the specified <paramref name="user"/>.</returns>
    string Hash(string target);

    /// <summary>
    /// Returns a <see cref="StringVerificationResult"/> indicating the result of a string hash comparison.
    /// </summary>
    /// <param name="alreadyHashedString">The hash value for a user's stored string.</param>
    /// <param name="targetString">The string supplied for comparison.</param>
    /// <returns>A <see cref="StringVerificationResult"/> indicating the result of a string hash comparison.</returns>
    /// <remarks>Implementations of this method should be time consistent.</remarks>
    HashedStringsVerificationResult VerifyHashedStrings(string alreadyHashedString, string targetString);
}