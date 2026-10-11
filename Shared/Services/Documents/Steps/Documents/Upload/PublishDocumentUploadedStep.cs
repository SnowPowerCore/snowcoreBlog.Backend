using MassTransit;
using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.Core.Contracts;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Publishes <see cref="DocumentUploaded"/> on RabbitMQ once a *new* document is fully
/// registered. Duplicate uploads do not re-announce the content — consumers can key
/// their own state on <see cref="DocumentUploaded.Checksum"/> when needed.
/// </summary>
public sealed class PublishDocumentUploadedStep(IPublishEndpoint publishEndpoint) : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public async Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        if (!context.IsDuplicate)
        {
            var document = context.Document!;
            await publishEndpoint.Publish<DocumentUploaded>(new(
                document.Id,
                document.FileName,
                document.ContentType,
                document.Category,
                document.Checksum,
                document.SizeBytes,
                document.OwnerId), token);
        }

        return await next(context, token);
    }
}
