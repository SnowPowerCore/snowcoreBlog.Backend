using Riok.Mapperly.Abstractions;
using snowcoreBlog.Backend.Email.Core.Contracts;
using snowcoreBlog.PublicApi.BusinessObjects.Dto;

namespace snowcoreBlog.Backend.ReadersManagement.Extensions;

[Mapper]
public static partial class EmailDomainExtensions
{
    public static partial CheckEmailDomain ToCheckEmailDomain(this CreateReaderAccountDto createReaderAccountDto);
}