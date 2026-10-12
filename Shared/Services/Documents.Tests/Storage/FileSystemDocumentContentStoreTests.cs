using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using snowcoreBlog.Backend.Documents.Options;
using snowcoreBlog.Backend.Documents.Services.Storage;

namespace snowcoreBlog.Backend.Documents.Tests.Storage;

public class FileSystemDocumentContentStoreTests : IDisposable
{
    private readonly string _rootPath;
    private readonly FileSystemDocumentContentStore _store;

    public FileSystemDocumentContentStoreTests()
    {
        _rootPath = Path.Combine(Path.GetTempPath(), "snowcore-doc-tests", Guid.NewGuid().ToString("N"));
        _store = new FileSystemDocumentContentStore(
            Microsoft.Extensions.Options.Options.Create(new DocumentsOptions
            {
                Storage = new DocumentsStorageOptions { RootPath = _rootPath }
            }),
            NullLogger<FileSystemDocumentContentStore>.Instance);
    }

    public void Dispose()
    {
        if (Directory.Exists(_rootPath))
        {
            Directory.Delete(_rootPath, recursive: true);
        }
    }

    private static MemoryStream Content(string text)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        return new MemoryStream(bytes);
    }

    [Fact]
    public async Task WriteAsync_ShouldCreateContentAddressableFile()
    {
        // Arrange
        var key = "aabbccdd00112233445566778899aabbccddeeff00112233445566778899aabb";

        // Act
        var resultKey = await _store.WriteAsync(Content("hello"), key);

        // Assert
        resultKey.Should().Be(key);
        (await _store.ExistsAsync(key)).Should().BeTrue();

        var expectedPath = Path.Combine(_rootPath, "aa", "bb", key);
        File.Exists(expectedPath).Should().BeTrue();
    }

    [Fact]
    public async Task WriteAsync_ShouldBeIdempotentForSameKey()
    {
        // Arrange
        var key = "11223344556677889900aabbccddeeff11223344556677889900aabbccddeeff";

        // Act
        await _store.WriteAsync(Content("first"), key);
        await _store.WriteAsync(Content("second"), key);
        await using var read = await _store.OpenReadAsync(key);
        using var reader = new StreamReader(read!);
        var text = await reader.ReadToEndAsync();

        // Assert
        text.Should().Be("first", "content-addressable storage keeps the first written blob");
    }

    [Fact]
    public async Task OpenReadAsync_UnknownKey_ShouldReturnNull()
    {
        var result = await _store.OpenReadAsync("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff");
        result.Should().BeNull();
    }

    [Fact]
    public async Task OpenReadAsync_ShouldReturnStoredBytes()
    {
        // Arrange
        var key = "0011223344556677889900aabbccddeeff00112233445566778899aabbccddeeff";
        await _store.WriteAsync(Content("payload-123"), key);

        // Act
        await using var read = await _store.OpenReadAsync(key);
        using var reader = new StreamReader(read!);
        var text = await reader.ReadToEndAsync();

        // Assert
        text.Should().Be("payload-123");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveContentAndStayIdempotent()
    {
        // Arrange
        var key = "99887766554433221100ffeeddccbbaa99887766554433221100ffeeddccbbaa";
        await _store.WriteAsync(Content("bye"), key);

        // Act
        await _store.DeleteAsync(key);
        var afterDelete = await _store.ExistsAsync(key);
        await _store.DeleteAsync(key); // second call must not throw

        // Assert
        afterDelete.Should().BeFalse();
    }
}
