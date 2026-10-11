using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using Microsoft.Extensions.Options;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.Backend.Documents.Extensions;
using snowcoreBlog.Backend.Documents.Options;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Steps.Documents.Upload;

/// <summary>
/// Terminal step of the upload pipeline: maps the stored entity to the public DTO and
/// decorates it with the absolute download URL built from <c>Documents:PublicBaseUrl</c>.
/// </summary>
public sealed class BuildUploadDocumentResultStep(IOptions<DocumentsOptions> options) : IStep<UploadDocumentDelegate, UploadDocumentContext, IMaybe<DocumentDto>>
{
    public Task<IMaybe<DocumentDto>> InvokeAsync(UploadDocumentContext context, UploadDocumentDelegate next, CancellationToken token = default)
    {
        var document = context.Document
            ?? throw new InvalidOperationException("Upload pipeline finished without a document record.");

        var dto = document.ToDto();
        dto.DownloadUrl = BuildDownloadUrl(options.Value.PublicBaseUrl, document.Id);

        return Task.FromResult(Maybe.Create(dto));
    }

    private static string BuildDownloadUrl(string publicBaseUrl, Guid id) =>
        $"{publicBaseUrl.TrimEnd('/')}/download/{id}";
}
