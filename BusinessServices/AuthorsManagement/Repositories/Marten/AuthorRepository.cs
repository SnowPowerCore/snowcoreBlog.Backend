using Marten;
using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.Backend.Infrastructure.Repositories.Marten.Base;
using snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;

namespace snowcoreBlog.Backend.AuthorsManagement.Repositories.Marten;

public class AuthorRepository(IDocumentSession session) : BaseMartenRepository<AuthorEntity>(session), IAuthorRepository
{
}
