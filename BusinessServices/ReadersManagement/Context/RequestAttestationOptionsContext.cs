using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class RequestAttestationOptionsContext(
    RequestAttestationOptionsForRegistrationDto requestAttestationOptionsForRegistration = null,
    RequestAttestationOptionsForLoginDto requestAttestationOptionsForLogin = null) : BaseGenericContext
{
    public RequestAttestationOptionsForRegistrationDto RequestAttestationOptionsForRegistration { get; } = requestAttestationOptionsForRegistration;

    public RequestAttestationOptionsForLoginDto RequestAttestationOptionsForLogin { get; } = requestAttestationOptionsForLogin;
}