using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class RequestAttestationOptionsContext(RequestAttestationOptionsDto requestAttestationOptions = null) : BaseGenericContext
{
    public RequestAttestationOptionsDto RequestAttestationOptions { get; } = requestAttestationOptions;
}