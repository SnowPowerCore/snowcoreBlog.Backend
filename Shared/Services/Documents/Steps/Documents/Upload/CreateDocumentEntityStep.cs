using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Entities.Documents;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Registers the document metadata in Marten. In duplicate mode the existing record is
/// kept untouched (idempotent uploads). New documents are marked <see cref="DocumentStatus.Processed"/>
/// right away because the current pipeline runs synchronously; async domain pipelines
/// (e.g. legal document OCR) can re-set the status to <see cref="DocumentStatus.Processing"/>
/// in their own steps instead.
/// </summary>
public sealed class CreateDocumentEntityStep(IDocumentRepository documentRepository) : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public async Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        if (context.IsDuplicate)
        {
            return await next(context, token);
        }

        var now = DateTime.UtcNow;

        var entity = new DocumentEntity
        {
            Id = Guid.NewGuid(),
            FileName = Path.GetFileName(context.FileName),
            ContentType = context.ContentType,
            SizeBytes = context.Content.CanSeek ? context.Content.Length : 0,
            Category = context.Category ?? "general",
            Status = DocumentStatus.Processed,
            Checksum = context.Checksum!,
            ContentKey = context.ContentKey!,
            OwnerId = context.OwnerId,
            Metadata = context.Metadata,
            UploadedAt = now,
            ProcessedAt = now
        };

        context.Document = await documentRepository.AddOrUpdateAsync(entity, entity.Id, true, token);

        return await next(context, token);
    }
}
