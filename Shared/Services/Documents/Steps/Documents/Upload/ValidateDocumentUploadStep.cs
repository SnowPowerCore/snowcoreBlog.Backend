using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.Backend.Documents.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Fail-fast gate of the upload pipeline: rejects empty content, oversized payloads
/// and disallowed content types before anything touches storage.
/// </summary>
public sealed class ValidateDocumentUploadStep(IOptions<DocumentsOptions> options) : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        var limits = options.Value.UploadLimits;

        if (context.Content is null || !context.Content.CanRead)
        {
            return Task.FromResult(DocumentUploadError<DocumentDto>.Create("Document content is required."));
        }

        if (string.IsNullOrWhiteSpace(context.FileName))
        {
            return Task.FromResult(DocumentUploadError<DocumentDto>.Create("FileName is required."));
        }

        if (context.Content.CanSeek && context.Content.Length == 0)
        {
            return Task.FromResult(DocumentUploadError<DocumentDto>.Create("Document content must not be empty."));
        }

        if (limits.MaxSizeBytes > 0 && context.Content.CanSeek && context.Content.Length > limits.MaxSizeBytes)
        {
            return Task.FromResult(DocumentUploadError<DocumentDto>.Create(
                $"Document exceeds the maximum allowed size of {limits.MaxSizeBytes} bytes."));
        }

        if (limits.AllowedContentTypes.Length > 0 &&
            !limits.AllowedContentTypes.Contains(context.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return Task.FromResult(DocumentUploadError<DocumentDto>.Create(
                $"Content type '{context.ContentType}' is not allowed."));
        }

        return next(context, token);
    }
}
