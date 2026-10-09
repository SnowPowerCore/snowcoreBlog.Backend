using MinimalStepifiedSystem.Base;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Context;

public class LoginByOneTimeCodeContext(VerifyOneTimeCodeDto verifyOneTimeCode = null) : BaseGenericContext
{
    public VerifyOneTimeCodeDto VerifyOneTimeCode { get; } = verifyOneTimeCode;
}
