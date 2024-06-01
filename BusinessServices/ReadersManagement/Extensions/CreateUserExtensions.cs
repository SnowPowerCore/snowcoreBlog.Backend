using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi;

namespace snowcoreBlog.Backend.ReadersManagement;

[Mapper]
public static partial class CreateUserExtensions
{
    public static partial CreateUser ToCreateUser(this CreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserExists ToValidateUserExists(this CreateReaderAccountDto createReaderAccountDto);

    public static ReaderEntity ToEntity(this CreateReaderAccountDto createReaderAccountDto, Guid userId) =>
        new()
        {
            UserId = userId,
            NickName = createReaderAccountDto.NickName
        };
}