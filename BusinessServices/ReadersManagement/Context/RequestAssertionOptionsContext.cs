using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class RequestAssertionOptionsContext(RequestAssertionOptionsDto requestAssertionOptions = null) : BaseGenericContext
{
    public RequestAssertionOptionsDto RequestAssertionOptions { get; } = requestAssertionOptions;
}