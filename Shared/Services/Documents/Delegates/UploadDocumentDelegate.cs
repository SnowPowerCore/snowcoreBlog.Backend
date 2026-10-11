using MaybeResults;
using snowcoreBlog.Backend.Documents.Context;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Delegates;

public delegate Task<IMaybe<DocumentDto>> UploadDocumentDelegate(UploadDocumentContext context, CancellationToken token = default);
