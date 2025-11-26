using Marten;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;

namespace snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;

public class AuthorRepository(IDocumentSession session) : BaseMartenRepository<AuthorEntity>(session), IAuthorRepository
{
}
