using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.Backend.AuthorsManagement.Delegates;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;

namespace snowcoreBlog.Backend.BusinessServices.AuthorsManagement.Steps;

public class CreateAuthorEntityForExistingUserStep : IStep<BecomeAuthorAccountDelegate, BecomeAuthorAccountContext, IMaybe<AuthorEntity>>
{
    private readonly IAuthorRepository _authorRepository;

    public CreateAuthorEntityForExistingUserStep(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<IMaybe<AuthorEntity>> InvokeAsync(BecomeAuthorAccountContext context, BecomeAuthorAccountDelegate next, CancellationToken cancellationToken = default)
    {
        var userId = context.UserId;
        var displayName = context.DisplayName;

        var existing = (await _authorRepository.GetAllAsync(cancellationToken)).FirstOrDefault(a => a.UserId == userId);
        if (existing != null)
            return Maybe.Create<AuthorEntity>(default!);

        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayName = displayName
        };
        await _authorRepository.AddOrUpdateAsync(author, saveChange: true, token: cancellationToken);
        return Maybe.Create(author);
    }
}
