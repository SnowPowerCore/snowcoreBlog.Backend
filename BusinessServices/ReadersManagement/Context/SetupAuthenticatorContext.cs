using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class SetupAuthenticatorContext(SetupReaderAccountAuthenticatorDto setupRequest = null) : BaseGenericContext
{
    public SetupReaderAccountAuthenticatorDto SetupRequest { get; } = setupRequest;
}
