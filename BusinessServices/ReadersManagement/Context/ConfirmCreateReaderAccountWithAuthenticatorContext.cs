using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class ConfirmCreateReaderAccountWithAuthenticatorContext(ConfirmCreateReaderAccountWithAuthenticatorDto request = null) : BaseGenericContext
{
    public ConfirmCreateReaderAccountWithAuthenticatorDto ConfirmRequest { get; } = request;
}
