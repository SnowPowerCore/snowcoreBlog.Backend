using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.Backend.Documents.Interfaces.Storage;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Persists the binary content via <see cref="IDocumentContentStore"/>. The store is
/// content-addressable, so the checksum doubles as the content key. Skipped when the
/// duplicate-resolution step already matched an existing document.
/// </summary>
public sealed class PersistDocumentContentStep(IDocumentContentStore contentStore) : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public async Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        if (context.IsDuplicate)
        {
            context.ContentKey = context.Document!.ContentKey;
            return await next(context, token);
        }

        context.ContentKey = await contentStore.WriteAsync(context.Content, context.Checksum!, token);

        return await next(context, token);
    }
}
