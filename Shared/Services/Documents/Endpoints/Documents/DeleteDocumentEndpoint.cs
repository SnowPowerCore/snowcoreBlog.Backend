using System.Net;
using System.Net.Mime;
using FastEndpoints;
using snowcoreBlog.Backend.Documents.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Documents.Endpoints.Documents;

/// <summary>
/// Soft-deletes a document record (Marten soft-delete policy). The binary content is
/// intentionally retained: content-addressable storage may be shared by multiple
/// document records, so blob deletion is a separate maintenance concern.
/// </summary>
public class DeleteDocumentEndpoint : EndpointWithoutRequest
{
    public required IDocumentRepository DocumentRepository { get; set; }

    public override void Configure()
    {
        Delete("{id:guid}");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        Description(b => b
            .WithTags(ApiTagConstants.Documents)
            .Produces((int)HttpStatusCode.NoContent)
            .Produces<ApiResponse>((int)HttpStatusCode.NotFound, MediaTypeNames.Application.Json)
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

        await DocumentRepository.RemoveAsync(document, true, ct);

        await Send.NoContentAsync(ct);
    }
}
