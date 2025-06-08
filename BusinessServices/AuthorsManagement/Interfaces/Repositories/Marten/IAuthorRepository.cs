using snowcoreBlog.Backend.Core.Entities.Author;
using snowcoreBlog.Backend.Core.Interfaces.Repositories;

namespace snowcoreBlog.Backend.AuthorsManagement.Interfaces.Repositories.Marten;

public interface IAuthorRepository : IRepository<AuthorEntity>
{
}
