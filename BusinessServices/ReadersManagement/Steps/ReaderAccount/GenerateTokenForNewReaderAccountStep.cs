using MassTransit;
using MinimalStepifiedSystem.Interfaces;
using Results;
using snowcoreBlog.Backend.ReadersManagement.Context;
using snowcoreBlog.Backend.ReadersManagement.Delegates;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Steps.ReaderAccount;

public class GenerateTokenForNewReaderAccountStep(IRequestClient<> client) : IStep<CreateReaderAccountDelegate, CreateReaderAccountContext, IResult<ReaderAccountCreationResultDto>>
{
    public async Task<IResult<ReaderAccountCreationResultDto>> InvokeAsync(CreateReaderAccountContext context, CreateReaderAccountDelegate next, CancellationToken token = default)
    {
        var jwtToken = JwtBearer.CreateToken(o =>
        {
            o.SigningKey = ;
            o.SigningStyle = TokenSigningStyle.Asymmetric;
            o.ExpireAt = ;
        });
        await next(context, token);
    }
}