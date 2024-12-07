using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class RequestCreateReaderAccountContext(RequestCreateReaderAccountDto request) : BaseGenericContext
{
    public RequestCreateReaderAccountDto Request { get; } = request;
}