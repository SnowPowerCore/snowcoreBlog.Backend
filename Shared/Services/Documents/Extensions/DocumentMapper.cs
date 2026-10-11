using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Documents;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.Documents.Extensions;

[Mapper]
public static partial class DocumentMapper
{
    public static partial DocumentDto ToDto(this DocumentEntity entity);

    public static partial IEnumerable<DocumentDto> ToDtos(this IEnumerable<DocumentEntity> entities);

    public static partial DocumentStatusDto ToStatusDto(this DocumentStatus status);
}
