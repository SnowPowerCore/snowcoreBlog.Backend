using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Core.Entities.Reader;
using snowcoreBlog.Backend.IAM.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement;

[Mapper]
public static partial class UserExtensions
{
    [MapProperty(nameof(ConfirmCreateReaderAccountDto.VerificationToken), nameof(CreateUser.TempUserVerificationToken))]
    public static partial CreateUser ToCreateUser(this ConfirmCreateReaderAccountDto createReaderAccountDto, string attestationOptionsJson);

    [MapProperty(nameof(RequestCreateReaderAccountDto.NickName), nameof(CreateTempUser.UserName))]
    public static partial CreateTempUser ToCreateTempUser(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static partial LoginUser ToLoginUser(this LoginByAssertionDto loginByAssertionDto, string assertionOptionsJson);

    public static partial ValidateUserExists ToValidateUserExists(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserExists ToValidateUserExists(this ConfirmCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserExists ToValidateUserExists(this LoginByAssertionDto createReaderAccountDto);

    public static partial ValidateTempUserExists ToValidateTempUserExists(this RequestCreateReaderAccountDto createReaderAccountDto);

    public static partial ValidateUserNickNameTaken ToValidateUserNickNameTaken(this RequestCreateReaderAccountDto createReaderAccountDto);

    [MapProperty(nameof(UserCreationResult.Id), nameof(ReaderEntity.UserId))]
    [MapProperty(nameof(UserCreationResult.UserName), nameof(ReaderEntity.NickName))]
    public static partial ReaderEntity ToEntity(this UserCreationResult userCreationResult);
}