using MassTransit;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.AuthorsManagement.Features;

public record CheckAuthorExistsRequest(Guid UserId);
public record CheckAuthorExistsResponse(bool Exists);

public class CheckAuthorExistsConsumer(IAuthorRepository authorRepository) : IConsumer<CheckAuthorExistsRequest>
{
    public async Task Consume(ConsumeContext<CheckAuthorExistsRequest> context)
    {
        var exists = (await authorRepository.GetAllAsync(context.CancellationToken)).Any(a => a.UserId == context.Message.UserId);
        await context.RespondAsync(new CheckAuthorExistsResponse(exists));
    }
}