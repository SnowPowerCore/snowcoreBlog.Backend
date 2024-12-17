using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class LoginByAssertionContext(LoginByAssertionDto loginByAssertion) : BaseGenericContext
{
    public LoginByAssertionDto LoginByAssertion { get; } = loginByAssertion;
}