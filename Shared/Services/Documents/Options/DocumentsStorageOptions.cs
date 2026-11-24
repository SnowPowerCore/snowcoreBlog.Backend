namespace snowcoreBlog.Backend.Documents.Options;

public record DocumentsStorageOptions
{
    /// <summary>
    /// Root folder of the default file-system content store. Relative paths are
    /// resolved against the application base directory. Swap the registered
    /// <c>IDocumentContentStore</c> implementation to move content to blob storage.
    /// </summary>
    public string? RootPath { get; init; }
}
