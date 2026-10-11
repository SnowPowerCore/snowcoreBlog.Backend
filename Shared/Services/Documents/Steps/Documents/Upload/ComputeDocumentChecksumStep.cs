using System.Security.Cryptography;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Computes the hex-encoded SHA-256 checksum of the content — the deduplication
/// identity for both the metadata store and the content store — and rewinds the
/// stream so subsequent steps can read the bytes again.
/// </summary>
public sealed class ComputeDocumentChecksumStep : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public async Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        if (!context.Content.CanSeek)
        {
            // The pipeline reads the content twice (hash + persist), so the stream must
            // be seekable — ASP.NET Core multipart streams always are.
            return DocumentUploadError<DocumentDto>.Create("Document content stream must be seekable.");
        }

        context.Checksum = await ComputeHexHashAsync(context.Content, token);
        context.Content.Position = 0;

        return await next(context, token);
    }

    private static async Task<string> ComputeHexHashAsync(Stream stream, CancellationToken token)
    {
        var hash = await SHA256.HashDataAsync(stream, token);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
