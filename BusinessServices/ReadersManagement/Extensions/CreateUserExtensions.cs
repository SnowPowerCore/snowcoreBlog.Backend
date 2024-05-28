using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

[Mapper]
public static partial class CreateUserExtensions
{
    public static partial CreateUser ToCreateUser(this CreateReaderAccountDto createReaderAccountDto);
}