using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.Backend.AuthorsManagement.Delegates;
using snowcoreBlog.Backend.AuthorsManagement.ErrorResults;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.AuthorsManagement.Steps;

public class CreateAuthorEntityForExistingUserStep(IAuthorRepository authorRepository) : IStep<BecomeAuthorAccountDelegate, BecomeAuthorAccountContext, IMaybe<AuthorDto>>
{
    public async Task<IMaybe<AuthorDto>> InvokeAsync(BecomeAuthorAccountContext context, BecomeAuthorAccountDelegate next, CancellationToken cancellationToken = default)
    {
        var userId = context.UserId;
        var displayName = context.DisplayName;

        if (string.IsNullOrWhiteSpace(displayName))
            return AuthorValidationError<AuthorDto>.Create("DisplayName is required");

        var exists = await authorRepository.AnyByUserIdAsync(userId, cancellationToken);
        if (exists)
            return AuthorAccountAlreadyExistsError<AuthorDto>.Create("Author account already exists");

        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayName = displayName
        };
        await authorRepository.AddOrUpdateAsync(author, saveChange: true, token: cancellationToken);

        return Maybe.Create(new AuthorDto
        {
            Id = author.Id,
            UserId = author.UserId,
            DisplayName = author.DisplayName
        });
    }
}