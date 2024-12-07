using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class GenerateTokenForNewReaderAccountStep() : IStep<RequestCreateReaderAccountDelegate, RequestCreateReaderAccountContext, IResult<RequestReaderAccountCreationResultDto>>
{
    public async Task<IResult<RequestReaderAccountCreationResultDto>> InvokeAsync(RequestCreateReaderAccountContext context, RequestCreateReaderAccountDelegate next, CancellationToken token = default)
    {
        // var jwtToken = JwtBearer.CreateToken(o =>
        // {
        //     o.SigningKey = ;
        //     o.SigningStyle = TokenSigningStyle.Asymmetric;
        //     o.ExpireAt = ;
        // });
        return await next(context, token);
    }
}