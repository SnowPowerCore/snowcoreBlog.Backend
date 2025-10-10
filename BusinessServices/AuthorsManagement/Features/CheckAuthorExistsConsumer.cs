using MassTransit;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.AuthorsManagement.Features;

public record CheckAuthorExistsRequest(Guid UserId);
public record CheckAuthorExistsResponse(bool Exists);

public class CheckAuthorExistsConsumer : IConsumer<CheckAuthorExistsRequest>
{
    private readonly IAuthorRepository _authorRepository;

    public CheckAuthorExistsConsumer(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task Consume(ConsumeContext<CheckAuthorExistsRequest> context)
    {
        var exists = (await _authorRepository.GetAllAsync(context.CancellationToken)).Any(a => a.UserId == context.Message.UserId);
        await context.RespondAsync(new CheckAuthorExistsResponse(exists));
    }
}