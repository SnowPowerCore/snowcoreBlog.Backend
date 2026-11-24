using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.ApiAccessRestrictions.Core.Entities.ApiAccessRestrictions;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Extensions;

[Mapper]
public static partial class ApiAccessMapper
{
    // Rules
    public static partial ApiAccessRuleDto ToDto(this ApiAccessRuleEntity entity);

    public static partial ApiAccessRuleEntity ToEntity(this CreateApiAccessRuleDto dto);

    [MapperIgnoreTarget(nameof(ApiAccessRuleEntity.Id))]
    [MapperIgnoreTarget(nameof(ApiAccessRuleEntity.CreatedAt))]
    public static partial void ApplyTo(this UpdateApiAccessRuleDto dto, ApiAccessRuleEntity entity);

    // Response templates
    public static partial ApiAccessResponseTemplateDto ToDto(this ApiAccessResponseTemplateEntity entity);

    public static partial ApiAccessResponseTemplateEntity ToEntity(this CreateApiAccessResponseTemplateDto dto);

    [MapperIgnoreTarget(nameof(ApiAccessResponseTemplateEntity.Id))]
    [MapperIgnoreTarget(nameof(ApiAccessResponseTemplateEntity.CreatedAt))]
    public static partial void ApplyTo(this UpdateApiAccessResponseTemplateDto dto, ApiAccessResponseTemplateEntity entity);
}
