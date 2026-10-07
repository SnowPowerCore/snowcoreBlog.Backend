using MaybeResults;
using MinimalStepifiedSystem.Interfaces;
using snowcoreBlog.Backend.AuthorsManagement.Context;
using snowcoreBlog.Backend.AuthorsManagement.Delegates;
using snowcoreBlog.Backend.AuthorsManagement.ErrorResults;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;
using snowcoreBlog.PublicApi.Services;

namespace snowcoreBlog.Backend.AuthorsManagement.Steps;

public class CreateAuthorEntityForExistingUserStep(
    IAuthorRepository authorRepository,
    IAuthorDisplayNameAvailabilityService authorDisplayNameAvailabilityService) : IStep<BecomeAuthorAccountDelegate, BecomeAuthorAccountContext, IMaybe<AuthorDto>>
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

        // Authoritative final check — the real-time indicator is advisory only;
        // a display name claimed between the last keystroke and this submit
        // must still be rejected here.
        var displayNameTaken = await authorRepository.AnyByDisplayNameAsync(displayName, cancellationToken);
        if (displayNameTaken)
            return AuthorDisplayNameNotAvailableError<AuthorDto>.Create("Display name is already taken");

        var author = new AuthorEntity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayName = displayName
        };
        await authorRepository.AddOrUpdateAsync(author, saveChange: true, token: cancellationToken);

        // Push the new availability state to every live watcher in real time:
        // invalidates the Fusion compute result for this display name, so any
        // connected client observing it flips from "Available" to "Taken".
        await authorDisplayNameAvailabilityService.NotifyReservedAsync(displayName, cancellationToken);

        return Maybe.Create(new AuthorDto
        {
            Id = author.Id,
            UserId = author.UserId,
            DisplayName = author.DisplayName
        });
    }
}