using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ApiAccessRestrictions.Context;

public class CreateApiAccessResponseTemplateContext(CreateApiAccessResponseTemplateDto request) : BaseGenericContext
{
    public CreateApiAccessResponseTemplateDto Request { get; } = request;
}
