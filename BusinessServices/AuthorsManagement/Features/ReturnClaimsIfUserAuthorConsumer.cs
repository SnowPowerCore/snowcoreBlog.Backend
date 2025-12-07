using MassTransit;
using snowcoreBlog.Backend.AspireYarpGateway.Core.Contracts;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.AuthorsManagement.Features;

public class ReturnClaimsIfUserAuthorConsumer(IAuthorRepository authorRepository) : IConsumer<RequestReaderClaims>
{
    public async Task Consume(ConsumeContext<RequestReaderClaims> context)
    {
        var exists = (await authorRepository.GetAllAsync(context.CancellationToken)).Any(a => a.UserId == context.Message.UserId);
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