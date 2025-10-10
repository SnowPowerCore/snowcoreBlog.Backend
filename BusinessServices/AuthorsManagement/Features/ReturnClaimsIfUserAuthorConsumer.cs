using MassTransit;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;

namespace snowcoreBlog.Backend.AuthorsManagement.Features;

public class ReturnClaimsIfUserAuthorConsumer : IConsumer<RequestReaderClaims>
{
    private readonly IAuthorRepository _authorRepository;

    public ReturnClaimsIfUserAuthorConsumer(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task Consume(ConsumeContext<RequestReaderClaims> context)
    {
        var exists = (await _authorRepository.GetAllAsync(context.CancellationToken)).Any(a => a.UserId == context.Message.UserId);
        await context.RespondAsync(new ReaderClaimsResponse
        {
            RequestId = context.Message.RequestId,
            SourceService = context.Message.TargetServiceName,
            Claims = exists
                ? new Dictionary<string, string> { ["authorAccount"] = true.ToString() }
                : []
        });
    }
}