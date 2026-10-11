namespace snowcoreBlog.Backend.Documents.Interfaces.Storage;

/// <summary>
/// Abstraction over the binary content storage used by the Documents service.
/// The default implementation (<c>FileSystemDocumentContentStore</c>) stores content
/// addressable by the SHA-256 checksum, so identical documents are physically stored once.
/// Swap the DI registration to move content to a blob provider (S3/Azure/etc.) without
/// touching any pipeline step — steps only ever see <see cref="ContentKey"/> strings.
/// </summary>
public interface IDocumentContentStore
{
    /// <summary>
    /// Persists the content under the given content key. Implementations must be
    /// idempotent: when content for the key already exists, it is kept as-is.
    /// </summary>
    /// <returns>The content key the content is retrievable with.</returns>
    Task<string> WriteAsync(Stream content, string contentKey, CancellationToken token = default);

    /// <summary>Opens the content for reading, or null when the key is unknown.</summary>
    Task<Stream?> OpenReadAsync(string contentKey, CancellationToken token = default);

    /// <summary>Checks whether content exists for the given key.</summary>
    Task<bool> ExistsAsync(string contentKey, CancellationToken token = default);

    /// <summary>Removes the content. Unknown keys are ignored (idempotent).</summary>
    Task DeleteAsync(string contentKey, CancellationToken token = default);
}
