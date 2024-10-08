using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class CreateReaderAccountContext(CreateReaderAccountDto request) : BaseGenericContext
{
    public CreateReaderAccountDto Request { get; } = request;
}