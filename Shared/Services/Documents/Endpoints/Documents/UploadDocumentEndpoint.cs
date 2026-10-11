using System.Net;
using System.Net.Mime;
using FastEndpoints;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using MinimalStepifiedSystem.Attributes;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.Backend.Documents.Delegates;
using snowcoreBlog.Backend.Documents.Steps.Documents.Upload;
using snowcoreBlog.Backend.Infrastructure;
using snowcoreBlog.Backend.Infrastructure.Utilities;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Constants;
using snowcoreBlog.PublicApi.Extensions;
using snowcoreBlog.PublicApi.Utilities.Api;

namespace snowcoreBlog.Backend.Documents.Endpoints.Documents;

/// <summary>
/// Uploads a new document (multipart/form-data).
/// The whole business flow is a StepifiedProcess — the processing sequence lives in
/// <see cref="UploadDocumentDelegate"/> and can be extended by inserting steps
/// (virus scan, OCR, thumbnailing, ...) into the <c>Steps</c> array below.
/// Form fields: <c>file</c> (required), <c>category</c> (optional), <c>ownerId</c> (optional),
/// any additional field is stored as document metadata.
/// </summary>
public class UploadDocumentEndpoint : EndpointWithoutRequest<ApiResponse?>
{
    private const string FileFormField = "file";
    private const string CategoryFormField = "category";
    private const string OwnerIdFormField = "ownerId";

    public required IOptions<JsonOptions> JsonOptions { get; set; }

    [StepifiedProcess(Steps = [
        typeof(ValidateDocumentUploadStep),
        typeof(ComputeDocumentChecksumStep),
        typeof(ResolveDuplicateDocumentStep),
        typeof(PersistDocumentContentStep),
        typeof(CreateDocumentEntityStep),
        typeof(PublishDocumentUploadedStep),
        typeof(BuildUploadDocumentResultStep),
    ])]
    protected UploadDocumentDelegate UploadDocument { get; } = default!;

    public override void Configure()
    {
        Post("upload");
        Version(1);
        SerializerContext(CoreSerializationContext.Default);
        AllowAnonymous();
        AllowFileUploads();
        Description(b => b
            .WithTags(ApiTagConstants.Documents)
            .Accepts<MultipartFormDataContent>(MediaTypeNames.Multipart.FormData)
            .Produces<ApiResponseForOpenApi<DocumentDto>>((int)HttpStatusCode.OK, MediaTypeNames.Application.Json)
            .Produces<ApiResponse>((int)HttpStatusCode.InternalServerError, MediaTypeNames.Application.Json)
            .ProducesProblemFE((int)HttpStatusCode.BadRequest));
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!HttpContext.Request.HasFormContentType)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["A multipart/form-data request with a 'file' field is required."],
                    (int)HttpStatusCode.BadRequest),
                (int)HttpStatusCode.BadRequest,
                ct);
            return;
        }

        var form = await HttpContext.Request.ReadFormAsync(ct);
        var file = form.Files.GetFiles(FileFormField).FirstOrDefault();

        if (file is null)
        {
            await Send.ResponseAsync(
                ErrorResponseUtilities.ApiResponseWithErrors(
                    ["The 'file' form field with the document content is required."],
                    (int)HttpStatusCode.BadRequest),
                (int)HttpStatusCode.BadRequest,
                ct);
            return;
        }

        var category = form[CategoryFormField].ToString();
        var ownerIdRaw = form[OwnerIdFormField].ToString();
        var metadata = form
            .Where(kv => kv.Key != FileFormField && kv.Key != CategoryFormField && kv.Key != OwnerIdFormField)
            .ToDictionary(static kv => kv.Key, static kv => kv.Value.ToString());

        await using var content = file.OpenReadStream();

        var context = new UploadDocumentContext(
            content,
            file.FileName,
            string.IsNullOrWhiteSpace(file.ContentType) ? MediaTypeNames.Application.Octet : file.ContentType,
            string.IsNullOrWhiteSpace(category) ? null : category,
            Guid.TryParse(ownerIdRaw, out var ownerId) ? ownerId : null,
            metadata.Count > 0 ? metadata : null);

        var result = await UploadDocument(context, ct);

        await Send.ResponseAsync(
            result?.ToApiResponse(serializerOptions: JsonOptions.Value.SerializerOptions),
            result?.ToStatusCode() ?? (int)HttpStatusCode.InternalServerError,
            ct);
    }
}
