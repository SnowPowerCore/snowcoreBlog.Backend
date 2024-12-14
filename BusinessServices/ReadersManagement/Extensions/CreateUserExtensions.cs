using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement;

[Mapper]
public static partial class CreateUserExtensions
{
    [MapProperty(nameof(RequestCreateReaderAccountDto.NickName), nameof(CreateTempUser.UserName))]
    public static partial CreateTempUser ToCreateTempUser(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserExists ToValidateUserExists(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserExists ToValidateUserExists(this ConfirmCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateTempUserExists ToValidateTempUserExists(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserNickNameTaken ToValidateUserNickNameTaken(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static ReaderEntity ToEntity(this RequestCreateReaderAccountDto createReaderAccountDto, Guid userId) =>
        new()
        {
            UserId = userId,
            NickName = createReaderAccountDto.NickName
        };
}