using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Documents.Interfaces.Storage;
using snowcoreBlog.Backend.Documents.Options;

namespace snowcoreBlog.Backend.Documents.Services.Storage;

/// <summary>
/// Default content-addressable file-system implementation of <see cref="IDocumentContentStore"/>.
/// Content lives under <c>{RootPath}/{key[0..2]}/{key[2..4]}/{key}</c> (two-level fan-out keeps
/// directories small). Writes are atomic (temp file + rename) so a crash never leaves a
/// truncated blob, and concurrent uploads of identical content converge on one file.
/// </summary>
public sealed class FileSystemDocumentContentStore(
    IOptions<DocumentsOptions> options,
    ILogger<FileSystemDocumentContentStore> logger) : IDocumentContentStore
{
    private readonly string _rootPath = ResolveRootPath(options.Value);

    public async Task<string> WriteAsync(Stream content, string contentKey, CancellationToken token = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(contentKey);

        var targetPath = GetContentPath(contentKey);

        if (File.Exists(targetPath))
        {
            // Content-addressable storage: identical key means identical bytes — keep the existing blob.
            return contentKey;
        }

        var directory = Path.GetDirectoryName(targetPath)!;
        Directory.CreateDirectory(directory);

        var tempPath = Path.Combine(directory, $".{Path.GetFileName(targetPath)}.tmp-{Guid.NewGuid():N}");
        try
        {
            await using (var target = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
            {
                await content.CopyToAsync(target, token);
            }

            File.Move(tempPath, targetPath, overwrite: false);
        }
        catch (IOException)
        {
            // Most likely a concurrent write of the same key won the rename race — that is fine,
            // the bytes are identical by construction (content-addressed).
            if (!File.Exists(targetPath))
            {
                throw;
            }

            logger.LogDebug("Concurrent write for content key '{ContentKey}' resolved to the existing blob.", contentKey);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }

        return contentKey;
    }

    public Task<Stream?> OpenReadAsync(string contentKey, CancellationToken token = default)
    {
        var path = GetContentPath(contentKey);
        return Task.FromResult<Stream?>(File.Exists(path)
            ? new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
            : null);
    }

    public Task<bool> ExistsAsync(string contentKey, CancellationToken token = default) =>
        Task.FromResult(File.Exists(GetContentPath(contentKey)));

    public Task DeleteAsync(string contentKey, CancellationToken token = default)
    {
        var path = GetContentPath(contentKey);
        if (File.Exists(path))
        {
            File.Delete(path);
        }

        return Task.CompletedTask;
    }

    private string GetContentPath(string contentKey)
    {
        // Content keys are hex checksums produced by the upload pipeline; defensive trimming
        // keeps the fan-out structure intact even for custom store implementations.
        var safeKey = contentKey.Replace("..", string.Empty).Replace('/', '_').Replace('\\', '_');
        var prefix = safeKey.Length >= 4 ? safeKey : safeKey.PadRight(4, '0');
        return Path.Combine(_rootPath, prefix[..2], prefix[2..4], safeKey);
    }

    private static string ResolveRootPath(DocumentsOptions options)
    {
        var configured = options.Storage?.RootPath;
        var root = string.IsNullOrWhiteSpace(configured)
            ? Path.Combine(AppContext.BaseDirectory, "document-content")
            : configured;

        return Path.GetFullPath(root);
    }
}
