using System.Net;
using System.Net.Mime;
using FastEndpoints;
using snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Documents.Interfaces.Storage;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Documents.Endpoints.Documents;

/// <summary>
/// Streams the binary content of a document. Content lookups are content-addressed,
/// so the stored original bytes (not a per-request copy) are served.
/// </summary>
public class DownloadDocumentEndpoint : EndpointWithoutRequest
{
    public required IDocumentRepository DocumentRepository { get; set; }

    public required IDocumentContentStore ContentStore { get; set; }

    public override void Configure()
    {
        Get("download/{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Documents)
            .Produces((int)HttpStatusCode.OK, contentType: MediaTypeNames.Application.Octet)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, contentType: MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<Guid>("id");
        var document = await DocumentRepository.GetByIdAsync(id, ct);

        if (document is default(Core.Entities.Documents.DocumentEntity))
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Document with ID '{id}' was not found."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        var content = await ContentStore.OpenReadAsync(document.ContentKey, ct);
        if (content is null)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    [$"Content of document '{id}' is not available anymore."],
                    (int)HttpStatusCode.NotFound),
                (int)HttpStatusCode.NotFound,
                ct);
            return;
        }

        // SendStreamAsync takes ownership and disposes the stream itself.
        await Send.StreamAsync(content, document.FileName, document.SizeBytes, document.ContentType, cancellation: ct);
    }
}
