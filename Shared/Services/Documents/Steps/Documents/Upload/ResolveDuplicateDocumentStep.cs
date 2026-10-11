using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Content-addressable deduplication: when a document with the same checksum already
/// exists the pipeline switches to "duplicate mode" — the content store write and the
/// <c>DocumentUploaded</c> publication are skipped downstream and the existing record
/// is returned instead. Extend or reorder this step when a domain service needs a
/// per-owner (instead of global) deduplication scope.
/// </summary>
public sealed class ResolveDuplicateDocumentStep(IDocumentRepository documentRepository) : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public async Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        var existing = await documentRepository.GetByChecksumAsync(context.Checksum!, token);
        if (existing is not null)
        {
            context.IsDuplicate = true;
            context.Document = existing;
        }

        return await next(context, token);
    }
}
